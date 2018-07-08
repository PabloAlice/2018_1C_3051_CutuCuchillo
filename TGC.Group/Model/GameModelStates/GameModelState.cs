using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.GameModelStates
{
    public abstract class GameModelState
    {
        public GameModelState() { }

        virtual public void Update() { }

        virtual public void Init() { }

        virtual public void Render() { }

        virtual public void Dispose() { }
    }
}
