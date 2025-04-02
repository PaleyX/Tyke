namespace Tyke.Net.Process
{
    internal class CommandModify() : CommandBase(CommandTypes.Modify)
    {
        private Data.DatafieldAlpha _datafield;
        private Misc.StringModifier _modifier;

        internal override CommandBase Process()
        {
            string text = _datafield.ToString();
            text = _modifier.Modify(text);
            _datafield.Set(text);

            return Next;
        }

        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            // at least 4 tokens
            if (stack.Count() < 4)
            {
                Errors.Error.SyntaxError();
                return;
            }

            // modify
            stack.VerifyAndPop("modify");

            // alpha datafield
            _datafield = Symbols.SymbolTable.GetSymbol<Data.DatafieldAlpha>(stack.Pop());
            if(_datafield == null)
                return;
            
            // set modifier
            _modifier = new Misc.StringModifier(_datafield.Length);

            // using
            stack.VerifyAndPop("using");

            // modifier list
            foreach (string token in stack.GetList())
            {
                _modifier.AddToken(token);
            }


    //// next words are the modifier list
    //eModToken emt;

    //while(tok.Count() > 0)
    //{
    //    switch(emt = ModifierToken(tok.Peek()))
    //    {
    //    case emtUppercase:
    //    case emtLowercase:
    //    case emtLeftJustify:
    //    case emtRightJustify:
    //    case emtCenter:
    //    case emtReverse:
    //    case emtNameCase:
    //    case emtRemoveConsecSpaces:
    //    case emtAddressCase:
    //    case emtShiftLeft:
    //    case emtShiftRight:
    //    case emtVowelReplacement:
    //    case emtRemoveRepeatingChars:
    //    case emtRemoveSpaces:
    //    case emtLetterMapping:
    //    case emtRemovePunctuation:
    //    case emtZeroTerminate:
    //    case emtUnZeroTerminate:
    //    case emtRemoveLeadingZeros:
    //    case emtRemoveRepeatingAlpha:
    //    case emtRemoveNonPrints:
    //    case emtCompanyCase:
    //        AddModifierToken(emt);
    //        break;
    //    case emtComma:
    //        break;
    //    default:
    //        CError::ReportError("Unknown modifier token: " + tok.Peek());
    //    }

    //    tok.Pop();
    //}

    //return true;
        }
    }
}
