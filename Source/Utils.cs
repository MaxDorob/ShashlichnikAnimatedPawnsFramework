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
            if (rootNode == null)
            {
                yield break;
            }
            yield return rootNode;
            if (rootNode.children != null)
            {
                foreach (var subNode in rootNode.children.SelectMany(AllRenderNodes))
                {
                    yield return subNode;
                }
            }
        }
    }
}
