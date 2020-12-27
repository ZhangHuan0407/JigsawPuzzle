using System;
using System.Text;

namespace JigsawPuzzle
{
    [ShareScript]
    [Serializable]
    public class ControllerAction
    {
        /* field */
        public string Action;
        public string Controller;
        public string[] FormKeys;
        public string[] FormValues;
        public string ReturnType;
        public string Type;

        /* ctor */
        public ControllerAction() { }

        /* operator */
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder()
                .AppendLine($"{nameof(Action)} : {Action}")
                .AppendLine($"{nameof(Controller)} : {Controller}")
                .AppendLine($"{nameof(FormKeys)} : {FormKeys} {FormKeys.Length}")
                .AppendLine($"{nameof(FormValues)} : {FormValues} {FormValues.Length}")
                .AppendLine($"{nameof(ReturnType)} : {ReturnType}")
                .AppendLine($"{nameof(Type)} : {Type}");
            return builder.ToString();
        }
    }
}