using System;
using System.Collections.Generic;
using System.Text;
using ScintillaNet;

namespace FDjpPlugin.Commands
{
    abstract class SCICommand : ICommand
    {
        protected ScintillaControl sci;

        public SCICommand(ScintillaControl scintillaControl){
            sci = scintillaControl;
        }

        public abstract void Execute();
    }
}
