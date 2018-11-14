using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MasterMindPlayer
{
    interface IPlayer<TMove,TResponse>
    {
        TMove Play(TResponse response);
        TMove Play();
    }
}
