using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tyke.Net.Process;

[UsedImplicitly]
[Command("display")]
internal class CommandDisplay : CommandBase
{
    private readonly List<Misc.DisplayItemBase> _items = [];

    internal override void ParseCommand(Parser.Tokeniser stack)
    {
        stack.VerifyAndPop("display");

        foreach (var token in stack.GetList())
        {
            if (Tools.StringTools.IsQuotedString(token))
            {
                var value = Tools.StringTools.GetQuotedString(token);

                _items.Add(new Misc.DisplayItemString(value));
            }
            else
            {
                var value = Symbols.SymbolTable.GetSymbol<Data.DatafieldBase>(token);
                if (value != null)
                    _items.Add(new Misc.DisplayItemField(value));
            }
        }
    }

    internal override CommandBase Process()
    {
        var sb = new StringBuilder();

        foreach (var item in _items)
        {
            sb.Append(item.Value());
        }

        Console.WriteLine(sb.ToString());

        return Next;
    }
}