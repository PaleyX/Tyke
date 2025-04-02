using System;
using System.Diagnostics;

namespace Tyke.Net.Process
{
    internal class CommandMove() : CommandBase(CommandTypes.Move)
    {
        private Data.OperandBase _source;
        private Data.DatafieldBase _object;

        private Data.OperandBinary _start;
        private Data.OperandBinary _length;

        internal override void ParseCommand(Parser.Tokeniser stack)
        {
            // 4 for standard or 8 for extended
            int count = stack.Count();
            if (!(count == 4 || count == 8))
            {
                Errors.Error.SyntaxError("Expected 4 or 8 tokens");
                return;
            }
            
            // first must be move
            stack.VerifyAndPop("move");
            
            // next is a literal or a datafield
            _source = Data.OperandHelpers.AllocateOpFromToken(stack.Pop());
            if(_source == null)
                return;

            // extended
            if (count == 8)
            {
                if (_source.DataType != Data.OperandBase.DataTypes.Datafield || _source is not Data.OperandAlpha)
                {
                    Errors.Error.SyntaxError("Extended move only supported on alpha datafields");
                }

                _start = new Data.OperandBinary();
                _length = new Data.OperandBinary();

                stack.VerifyAndPop(":");
                _start.SetFromToken(stack.Pop());
                stack.VerifyAndPop(":");
                _length.SetFromToken(stack.Pop());
            }

            // next is 'to
            stack.VerifyAndPop("to");

            // next must be a variable
            _object = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(stack.Pop(), false);
            if (_object == null)
                Errors.Error.SyntaxError(Errors.StdErrors.ExpectedDatafield);

            if(_object != null && _source != null)
                Data.OperandHelpers.Compatible(_source, _object);
        }

        internal override CommandBase Process()
        {
            Debug.Assert(_source != null);
            Debug.Assert(_object != null);
            Debug.Assert(Data.OperandHelpers.Compatible(_source, _object));

            if (_object is Data.DatafieldBinary binary)
            {
                binary.Set(_source as Data.OperandBinary);
            }
            else
            {
                if (_start != null || _length != null)
                {
                    ExtendedMove();
                }
                else
                {
                    (_object as Data.DatafieldAlpha).Set(_source as Data.OperandAlpha);
                }
            }

            return Next;
        }

        private void ExtendedMove()
        {
            //TODO: ExtendedMove
            throw new NotImplementedException();
        }
    }
}
