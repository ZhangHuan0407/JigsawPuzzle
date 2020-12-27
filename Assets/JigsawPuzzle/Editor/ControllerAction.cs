using System;
using System.Collections;
using System.Collections.Generic;

namespace JigsawPuzzle
{
    [ShareScript]
    [Serializable]
    public class ControllerAction : IEnumerable<KeyValuePair<string, string>>
    {
        /* const */

        /* field */
        public string ReturnType;
        public Dictionary<string, string> Form;
        public string Type;

        /* ctor */
        public ControllerAction() { }

        /* IEnumerable */
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator() => Form.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}