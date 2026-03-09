using System.Globalization;
using System.Text;

namespace RepublicTK.Scripts
{
    public class Script
    {
        const char TokenIdentifier = '$';

        private int _cursorPosition;
        private int _selectionStart;
        private int _selectionEnd;

        public string ScriptContent { get; private set; }

        public Script(string scriptContent)
        {
            ScriptContent = scriptContent;
        }

        public string CurrentSelection
        {
            get
            {
                if (_selectionEnd <= _selectionStart)
                {
                    return "";
                }

                return ScriptContent[_selectionStart.._selectionEnd];
            }
        }

        public void Prepend(string tokenName, params object[] values)
        {
            Insert(_selectionStart, GetPropertyString(tokenName, values));
        }

        public void Append(string tokenName, params object[] values)
        {
            Insert(_selectionEnd, GetPropertyString(tokenName, values));
        }

        public void RemoveSelection()
        {
            if (_selectionEnd <= _selectionStart)
            {
                return;
            }

            ScriptContent = ScriptContent.Remove(_selectionStart, _selectionEnd - _selectionStart);

            _selectionEnd = _selectionStart;
            _cursorPosition = _selectionStart;
        }

        public bool NextToken(out string tokenName)
        {
            _cursorPosition = _selectionEnd;

            while (_cursorPosition < ScriptContent.Length)
            {
                if (ReadNextBlock(out tokenName) && IsValidTokenName(tokenName))
                {
                    int blockStart = _cursorPosition - tokenName.Length;
                    if (IsLineStart(blockStart))
                    {
                        _selectionStart = blockStart;
                        _selectionEnd = _cursorPosition;
                        return true;
                    }
                }
            }

            tokenName = "";
            return false;
        }

        public bool PreviousToken(out string tokenName)
        {
            _cursorPosition = _selectionStart;

            while (_cursorPosition > 0)
            {
                if (ReadPreviousBlock(out tokenName) && IsValidTokenName(tokenName))
                {
                    int blockStart = _cursorPosition;
                    if (IsLineStart(blockStart))
                    {
                        _selectionStart = blockStart;
                        _selectionEnd = blockStart + tokenName.Length;
                        return true;
                    }
                }
            }

            tokenName = "";
            return false;
        }

        public bool FindFirstToken(string tokenName)
        {
            _cursorPosition = 0;
            _selectionStart = 0;
            _selectionEnd = 0;

            return FindNextToken(tokenName);
        }

        public bool FindLastToken(string tokenName)
        {
            _cursorPosition = ScriptContent.Length;
            _selectionStart = ScriptContent.Length;
            _selectionEnd = ScriptContent.Length;

            return FindPreviousToken(tokenName);
        }

        public bool FindNextToken(string tokenName)
        {
            if (!IsValidTokenName(tokenName))
            {
                throw new ArgumentException("Token names must be at least two characters long, start with a token identifier, and consist of only uppercase ASCII letters, digits, and underscores.", nameof(tokenName));
            }

            int selectionStart = _selectionStart;
            int selectionEnd = _selectionEnd;

            while (NextToken(out var token))
            {
                if (token.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            _selectionStart = selectionStart;
            _selectionEnd = selectionEnd;

            return false;
        }

        public bool FindPreviousToken(string tokenName)
        {
            if (!IsValidTokenName(tokenName))
            {
                throw new ArgumentException("Token names must be at least two characters long, start with a token identifier, and consist of only uppercase ASCII letters, digits, and underscores.", nameof(tokenName));
            }

            int selectionStart = _selectionStart;
            int selectionEnd = _selectionEnd;

            while (PreviousToken(out var token))
            {
                if (token.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            _selectionStart = selectionStart;
            _selectionEnd = selectionEnd;

            return false;
        }

        public bool ReadValue<T>(uint offset, out T? value)
            where T: IParsable<T>
        {
            for (int i = 0; i < offset; i++)
            {
                if (!ReadValue<string>(out _))
                {
                    value = default;
                    return false;
                }
            }

            return ReadValue(out value);
        }

        public bool ReadValue<T>(out T? value) where T : IParsable<T>
        {
            _cursorPosition = _selectionEnd;

            if (ReadNextBlock(out string content))
            {
                int blockStart = _cursorPosition - content.Length;
                if (IsValidTokenName(content) && IsLineStart(blockStart))
                {
                    value = default;
                    return false;
                }

                if (typeof(T) == typeof(string))
                {
                    content = content.Trim('"');
                }

                if (T.TryParse(content, CultureInfo.InvariantCulture, out value))
                {
                    _selectionEnd = _cursorPosition;
                    return true;
                }
            }

            value = default;
            return false;
        }

        private void Insert(int index, string property)
        {
            char? charBefore = ScriptContent.ElementAtOrDefault(index - 1);
            char? charAfter = ScriptContent.ElementAtOrDefault(index);

            if (charBefore != '\n')
            {
                property = '\n' + property;
            }

            if (!(charAfter == '\n' || charAfter == '\r'))
            {
                property = property + '\n';
            }

            ScriptContent = ScriptContent.Insert(index, property);
            _selectionStart = index;
            _selectionEnd = index + property.Length;
        }

        private string GetPropertyString(string tokenName, object[] values)
        {
            if (!IsValidTokenName(tokenName))
            {
                throw new ArgumentException("Token names must be at least two characters long, start with a token identifier, and consist of only uppercase ASCII letters, digits, and underscores.", nameof(tokenName));
            }

            var builder = new StringBuilder(tokenName);
            builder.Append(' ');
            builder.AppendJoin(' ', values.Select(x =>
            {
                if (x is IFormattable formattable)
                {
                    return formattable.ToString(null, CultureInfo.InvariantCulture);
                }

                return x.ToString();
            }));

            return builder.ToString();
        }

        private bool ReadNextBlock(out string content)
        {
            int blockStart = FindNextIndex(_cursorPosition);
            if (blockStart == -1)
            {
                _cursorPosition = ScriptContent.Length;
                content = "";
                return false;
            }

            _cursorPosition = blockStart;

            if (ScriptContent[_cursorPosition] == '"')
            {
                int closingIndex = FindNextIndex('"', _cursorPosition + 1);
                if (closingIndex == -1)
                {
                    throw new InvalidOperationException("Unmatched quotation mark in script content.");
                }

                _cursorPosition = closingIndex + 1;
            }
            else
            {
                int blockEnd = FindNextWhiteSpace(_cursorPosition);
                if (blockEnd == -1)
                {
                    blockEnd = ScriptContent.Length;
                }
                
                _cursorPosition = blockEnd;
            }

            content = ScriptContent[blockStart.._cursorPosition];
            return true;
        }

        private bool ReadPreviousBlock(out string content)
        {
            int blockEnd = FindPreviousIndex(_cursorPosition - 1);
            if (blockEnd == -1)
            {
                _cursorPosition = 0;
                content = "";
                return false;
            }

            _cursorPosition = blockEnd;

            if (ScriptContent[_cursorPosition] == '"')
            {
                int opening = FindPreviousIndex('"', _cursorPosition - 1);
                if (opening == -1)
                {
                    throw new InvalidOperationException("Unmatched quotation mark in script content.");
                }

                _cursorPosition = opening;
            }
            else
            {
                int blockStart = FindPreviousWhiteSpace(_cursorPosition);
                if (blockStart == -1)
                {
                    blockStart = 0;
                }

                _cursorPosition = blockStart;
            }

            content = ScriptContent[_cursorPosition..blockEnd];
            return true;
        }

        private int FindNextIndex(int startPosition)
        {
            for (int i = startPosition; i < ScriptContent.Length; i++)
            {
                if (!char.IsWhiteSpace(ScriptContent[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindNextIndex(char c, int startPosition)
        {
            for (int i = startPosition; i < ScriptContent.Length; i++)
            {
                if (c == ScriptContent[i])
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindNextWhiteSpace(int startPosition)
        {
            for (int i = startPosition; i < ScriptContent.Length; i++)
            {
                if (char.IsWhiteSpace(ScriptContent[i]))
                {
                    return i;
                }
            }

            return -1;
        }

        private int FindPreviousIndex(int startPosition)
        {
            for (int i = startPosition - 1; i >= 0; i--)
            {
                if (!char.IsWhiteSpace(ScriptContent[i]))
                {
                    return i + 1;
                }
            }

            return -1;
        }

        private int FindPreviousIndex(char c, int startPosition)
        {
            for (int i = startPosition - 1; i >= 0; i--)
            {
                if (c == ScriptContent[i])
                {
                    return i + 1;
                }
            }

            return -1;
        }

        private int FindPreviousWhiteSpace(int startPosition)
        {
            for (int i = startPosition - 1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(ScriptContent[i]))
                {
                    return i + 1;
                }
            }

            return -1;
        }

        private bool IsValidTokenName(string? name)
        {
            return name?.Length >= 2 && name[0] == TokenIdentifier &&
                name.Skip(1).All(c => char.IsAsciiLetterUpper(c) || char.IsDigit(c) || c == '_');
        }

        private bool IsLineStart(int index)
        {
            int selectedLineStart = FindPreviousIndex('\n', index);
            if (selectedLineStart == -1)
            {
                selectedLineStart = 0;
            }

            return ScriptContent[selectedLineStart..index]
                .All(x => char.IsWhiteSpace(x));
        }
    }
}
