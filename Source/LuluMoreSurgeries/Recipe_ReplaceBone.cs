﻿using RimWorld;
using System.Collections.Generic;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.MoreSurgeries
{
    public class Recipe_ReplaceBone : Recipe_InstallArtificialBodyPart
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            // Check all of our hediffs for damage.
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                // Is the hediff a missing part visible on a natural solid body part that can be amputated?
                if (hediff is Hediff_MissingPart && hediff.Visible && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(hediff.Part) && (hediff.Part?.def.canSuggestAmputation ?? false) && (hediff.Part?.def.IsSolid(hediff.Part, hediff.pawn.health.hediffSet.hediffs) ?? false))
                {
                    // It is, so yield the damaged part.
                    yield return hediff.Part;
                }
            }
            // We're done.
            yield break;
        }
    }
}
