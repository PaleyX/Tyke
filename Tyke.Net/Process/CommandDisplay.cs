using System;
using System.Collections.Generic;
using System.Text;

namespace Tyke.Net.Process;

internal class CommandDisplay() : CommandBase(CommandTypes.Display)
{
    private readonly List<Misc.DisplayItemBase> _items = [];

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        stack.VerifyAndPop("display");

        foreach (string token in stack.GetList())
        {
            if (Tools.StringTools.IsQuotedString(token))
            {
                string value = Tools.StringTools.GetQuotedString(token);

                _items.Add(new Misc.DisplayItemString(value));
            }
            else
            {
                var value = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(token);
                if (value != null)
                    _items.Add(new Misc.DisplayItemField(value));
            }
        }

        //int count = 1;
        //while (stack.Count() > 0)
        //{
        //    if (count % 2 == 0)
        //        stack.VerifyAndPop(",");
        //    else
        //    {
        //        string token = stack.Pop();

        //        if (Tools.StringTools.IsQuotedString(token))
        //        {
        //            string value = Tools.StringTools.GetQuotedString(token);

        //            _Items.Add(new Misc.DisplayItemString(value));
        //        }
        //        else
        //        {
        //            var value = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(token);
        //            if (value != null)
        //                _Items.Add(new Misc.DisplayItemField(value));
        //        }
        //    }

        //    ++count;
        //}
    }

    internal override CommandBase Process()
    {
        var sb = new StringBuilder();

        foreach (Misc.DisplayItemBase item in _items)
        {
            sb.Append(item.Value());
        }

        Console.WriteLine(sb.ToString());

        return Next;
    }
}