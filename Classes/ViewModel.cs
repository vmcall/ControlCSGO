using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control.Classes
{
    public class ViewModel
    {
        IntPtr entPointer;

        public ViewModel(IntPtr viewModelPointer)
        {
            this.entPointer = viewModelPointer;
        }

        public int ModelIndex
        {
            get
            {
                return M.Read<int>(this.entPointer + Offsets.m_nModelIndex);
            }
            set
            {
                M.Write(value, this.entPointer + Offsets.m_nModelIndex);
            }
        }
    }
}
