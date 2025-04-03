using System;
using System.Collections.Generic;

namespace Tyke.Net.Misc;

internal class StringModifier
{
    private readonly List<Action> _modifiers = [];
    private string _text;
    private readonly int _length;

    internal StringModifier(int length)
    {
        _length = length;
    }

    internal string Modify(string text)
    {
        _text = text;

        foreach (Action action in _modifiers)
        {
            action();
        }

        return _text;
    }

    internal void AddToken(string token)
    {
        switch (token)
        {
            case "uc":
                _modifiers.Add(ToUpper);
                break;
            case "lc":
                _modifiers.Add(ToLower);
                break;
            case "rj":
                _modifiers.Add(RightJustify);
                break;
            case "lj":
                _modifiers.Add(LeftJustify);
                break;
            case "ac":
                _modifiers.Add(AddressCase);
                break;
            case "nc":
                _modifiers.Add(NameCase);
                break;
            case "sl":
                _modifiers.Add(ShiftLeft);
                break;
            case "sr":
                _modifiers.Add(ShiftRight);
                break;
            case "rlz":
                _modifiers.Add(RemoveLeadingZeros);
                break;
            case "rs":
                _modifiers.Add(RemoveSpaces);
                break;
            default:
                Errors.Error.SyntaxError("Unknown modify token: {0}", token);
                break;
        }
    }

    private void ToUpper()
    {
        _text = _text.ToUpper();
    }

    private void ToLower()
    {
        _text = _text.ToLower();
    }

    private void RightJustify()
    {
        _text = _text.PadLeft(_length);
    }

    private void LeftJustify()
    {
        _text = _text.TrimStart();
    }

    private void AddressCase()
    {
        //TODO: Address case
        throw new NotImplementedException("AddressCase");
    }

    private void NameCase()
    {
        //TODO: Name case
        throw new NotImplementedException("NameCase");
    }

    private void ShiftLeft()
    {
        _text = _text.Substring(1);
    }

    private void ShiftRight()
    {
        _text = " " + _text;

        if (_text.Length > _length)
            _text = _text.Substring(0, _length);
    }

    private void RemoveLeadingZeros()
    {
        _text = _text.TrimStart('0');
    }

    private void RemoveSpaces()
    {
        _text = _text.Replace(" ", "");
    }
}