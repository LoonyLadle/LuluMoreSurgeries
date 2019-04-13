using RimWorld;
using System.Collections.Generic;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.MoreSurgeries
{
    public class Recipe_InstallCosmeticBodyPart : Recipe_InstallArtificialBodyPart
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            // Check all of our hediffs for disfigurement.
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                // Is the hediff a permanent injury or missing part visible on a part that causes disfigurement?
                if (((hediff is Hediff_Injury && hediff.IsPermanent()) || hediff is Hediff_MissingPart) && hediff.Visible && (hediff.Part?.def.beautyRelated ?? false))
                {
                    // It is, so yield the disfigured part.
                    yield return hediff.Part;
                }
            }
            // We're done.
            yield break;
        }
    }
}
