using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Shashlichnik
{
    internal static class Utils
    {
        internal static IEnumerable<PawnRenderNode> AllRenderNodes(this PawnRenderNode rootNode)
        {
            yield return rootNode;
            foreach (var subNode in rootNode.children.SelectMany(x => AllRenderNodes(x)))
            {
                yield return subNode;
            }
        }
    }
}
