using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Control
{
    public interface IRayCastable
    {
        CursorType GetCursorType();
        bool HandleRaycast(PlayerController callingController);
    }
}
