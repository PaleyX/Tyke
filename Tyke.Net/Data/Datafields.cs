namespace Tyke.Net.Data
{
    internal static class Datafields
    {
        internal static void Compile()
        {
            int start = 1;

            while (true)
            {
                string line = Parser.TykeFile.GetLine();

                if (line.ToLower() == "end")
                    break;

                DatafieldBase datafield = CompileDatafield(line, start);

                if (datafield != null)
                    start = datafield.Offset + 1 +  datafield.Length;
            }
        }

        private static DatafieldBase CompileDatafield(string line, int start)
        {
            var tokeniser = new Parser.Tokeniser(line);

            string name = null;
            string offset = null;
            string length = null;
            string type = "a";
            string constant = null;

            int index = 1;
            bool gotType = false;
            bool gotConst = false;

            while(tokeniser.Count() > 0)
            {
                string token = tokeniser.Pop();

                if (token == "/")
                {
                    ++index;
                    continue;
                }

                switch(index)
			    {
			        // datafield name
			        case 1:
				        if(!string.IsNullOrWhiteSpace(name))
                            Errors.Error.SyntaxError();
				        name = token;
				        ++index;
				        break;
			        // must be '='
			        case 2:
				        if(token != "=") 
                            Errors.Error.SyntaxError();
				        ++index;
				        break;
			        // offset
			        case 3:
				        if(!string.IsNullOrWhiteSpace(offset)) 
                            Errors.Error.SyntaxError();
                        offset = token == "*" ? start.ToString() : token;
				        break;
			        // length
			        case 4:
				        if(!string.IsNullOrWhiteSpace(length))
                            Errors.Error.SyntaxError();
				        length = token;
				        break;
			        // type
			        case 5:
				        if(gotType) 
                            Errors.Error.SyntaxError();
				        type = token;
				        gotType = true;
				        break;
			        // unused
			        case 6:
				        Errors.Error.SyntaxError("Unused element");
				        break;
			        // const
			        case 7:
				        if(!string.IsNullOrWhiteSpace(constant))
                            Errors.Error.SyntaxError();
				        constant = token;
				        gotConst = true;
				        break;
			        // what?
			        default:
				        Errors.Error.SyntaxError("Unknown datafield element: " + token);
                        break;
			    }
            }

            DatafieldBase datafield = null;

            switch (type)
            {
                case "a":
                    datafield = new DatafieldAlpha(DataBuffer.CurrentBuffer, name, offset, length);
                    break;
                case "b":
                    datafield = new DatafieldBinary(DataBuffer.CurrentBuffer, name, offset, length);
                    break;
                default:
                    Errors.Error.SyntaxError("Unsupported field type [{0}]", type);
                    break;
            }

            // check and validate
            if (datafield != null)
            {
                if (datafield.CheckAndValidateElements())
                {
                    if (gotConst)
                        datafield.SetConstant(constant);
                }
            }

            return datafield;
        }
    }
}
