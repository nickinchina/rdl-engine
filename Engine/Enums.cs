using System;
using System.Collections.Generic;
using System.Text;

namespace Rdl.Engine
{
    public class Enums
    {
        public enum Auto
        {
            True,
            False,
            Auto
        };

        public enum DataElementOutputEnum
        {
            Output,
            NoOutput,
            ContentsOnly,
            Auto
        };

        public enum ToggleDirectionEnum
        {
            positive,
            negative
        };
    }
}
