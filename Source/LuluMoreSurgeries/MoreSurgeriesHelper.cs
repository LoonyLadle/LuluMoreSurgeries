using System.Collections.Generic;
using System.Linq;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.MoreSurgeries
{
    internal static class MoreSurgeriesHelper
    {
        public static IEnumerable<BodyPartRecord> GetPartsPossibleToInstallArtificialPart(Pawn pawn, RecipeDef recipe)
        {
            foreach (BodyPartRecord record in pawn.RaceProps.body.AllParts)
            {
                IEnumerable<Hediff> diffs = pawn.health.hediffSet.hediffs.Where(x => x.Part == record);
                if (diffs.Count() != 1 || diffs.First().def != recipe.addsHediff)
                {
                    if (record.parent == null || pawn.health.hediffSet.GetNotMissingParts().Contains(record.parent))
                    {
                        if (!pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) || pawn.health.hediffSet.HasDirectlyAddedPartFor(record))
                        {
                            yield return record;
                        }
                    }
                }
            }
            yield break;
        }
    }
}
