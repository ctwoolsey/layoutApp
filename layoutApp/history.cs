using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace layoutApp
{

    public class history
    {
        private List<Cursor> _previousCursor = new List<Cursor>();
        private List<string> _previousLayoutActionState = new List<string>();
        private string _defaultLayoutActionState;

        public history(string defaultLayoutActionState)
        {
            _defaultLayoutActionState = defaultLayoutActionState;

        }

        public history()
        {

        }

        public string layoutActionState
        {
            get { 
                string returnState;
                if (_previousLayoutActionState.Count > 0)
                {
                    returnState = _previousLayoutActionState[_previousLayoutActionState.Count - 1];
                    _previousLayoutActionState.RemoveAt(_previousLayoutActionState.Count - 1);
                }
                else
                {
                    returnState = _defaultLayoutActionState;
                }
                return (returnState);
            }
            set { _previousLayoutActionState.Add(value); }
        }

        public Cursor layoutCursor
        {
            get
            {
                Cursor returnCursor = Cursors.Default;
                if (_previousCursor.Count > 0)
                {
                    returnCursor = _previousCursor[_previousCursor.Count - 1];
                    _previousCursor.RemoveAt(_previousCursor.Count - 1);
                }

                return (returnCursor);
            }
            set { _previousCursor.Add(value); }
        }
    }
}
