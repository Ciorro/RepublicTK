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

        public bool NextToken(out string token)
        {
            _cursorPosition = _selectionEnd;

            while (_cursorPosition < ScriptContent.Length)
            {
                if (ReadNextBlock(out token) && token[0] == TokenIdentifier)
                {
                    _selectionStart = _cursorPosition - token.Length;

                    if (IsLineStart(_selectionStart))
                    {
                        return true;
                    }
                }
            }

            token = "";
            return false;
        }

        public bool PreviousToken(out string tokenName)
        {
            _cursorPosition = _selectionStart;

            while (_cursorPosition > 0)
            {
                if (ReadPreviousBlock(out tokenName) && tokenName[0] == TokenIdentifier)
                {
                    _selectionEnd = _cursorPosition + tokenName.Length;

                    if (IsLineStart(_selectionStart))
                    {
                        return true;
                    }
                }
            }

            tokenName = "";
            return false;
        }

        public bool FindToken(string tokenName)
        {
            _cursorPosition = 0;
            _selectionStart = 0;
            _selectionEnd = 0;

            return FindNextToken(tokenName);
        }

        public bool FindNextToken(string tokenName)
        {
            if (!IsValidTokenName(tokenName))
            {
                throw new ArgumentException("Token names must be at least two characters long, start with a token identifier, and consist of only uppercase ASCII letters, digits, and underscores.", nameof(tokenName));
            }

            while (NextToken(out var token))
            {
                if (token.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public bool FindPreviousToken(string tokenName)
        {
            if (!IsValidTokenName(tokenName))
            {
                throw new ArgumentException("Token names must be at least two characters long, start with a token identifier, and consist of only uppercase ASCII letters, digits, and underscores.", nameof(tokenName));
            }

            while (PreviousToken(out var token))
            {
                if (token.Equals(tokenName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        public T ReadValue<T>(uint offset = 0) where T : IParsable<T>
        {
            for (int i = 0; i < offset; i++)
            {
                ReadValue<string>();
            }

            if (ReadNextBlock(out string content))
            {
                if (IsLineStart(_cursorPosition - content.Length) && IsValidTokenName(content))
                {
                    throw new InvalidOperationException("Cannot read beyond the end of the current property.");
                }

                if (typeof(T) == typeof(string))
                {
                    content = content.Trim('"');
                }

                return T.Parse(content, CultureInfo.InvariantCulture);
            }

            throw new InvalidOperationException("No more blocks to read.");
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
            _cursorPosition = _selectionEnd;

            int blockStart = FindNextIndex(_cursorPosition);
            if (blockStart == -1)
            {
                _cursorPosition = ScriptContent.Length;
                content = "";
                return false;
            }

            int blockEnd = blockStart;
            _cursorPosition = blockStart;

            if (ScriptContent[_cursorPosition] == '"')
            {
                blockEnd = FindNextIndex('"', blockStart) + 1;
            }
            else
            {
                blockEnd = FindNextWhiteSpace(blockStart);
            }

            if (blockEnd == -1)
            {
                _selectionEnd = ScriptContent.Length;
                _cursorPosition = ScriptContent.Length;

                content = "";
                return false;
            }

            _selectionEnd = blockEnd;
            _cursorPosition = blockEnd;

            content = ScriptContent[blockStart..blockEnd];
            return true;
        }

        private bool ReadPreviousBlock(out string content)
        {
            _cursorPosition = _selectionStart;

            int blockEnd = FindPreviousIndex(_cursorPosition);
            if (blockEnd == -1)
            {
                _cursorPosition = 0;
                content = "";
                return false;
            }

            int blockStart = blockEnd;
            _cursorPosition = blockEnd;

            if (ScriptContent[_cursorPosition] == '"')
            {
                blockStart = FindPreviousIndex('"', blockEnd - 1);
            }
            else
            {
                blockStart = FindPreviousWhiteSpace(blockEnd);
            }

            if (blockStart == -1)
            {
                _selectionStart = 0;
                _cursorPosition = 0;

                content = "";
                return false;
            }

            _selectionStart = blockStart;
            _cursorPosition = blockStart;

            content = ScriptContent[blockStart..blockEnd];
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

        private bool IsValidTokenName(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                ArgumentException.ThrowIfNullOrEmpty(name);
            }

            return name.Length >= 2 && name[0] == TokenIdentifier &&
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
