using System.Threading.Tasks;
using UnityEngine;

namespace Simulation
{
    public abstract class Service : ScriptableObject
    {
        public abstract Task Activate();
    }
}