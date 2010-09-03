using System;
using System.Collections.Generic;
using System.Text;
using ScintillaNet;

namespace FDjpPlugin.Commands
{
    public interface ICommand
    {
        void Execute();
    }
}
