using RimWorld;
using System.Collections.Generic;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.MoreSurgeries
{
    public class Recipe_RemoveScar : Recipe_Surgery
    {
        // Is the hediff a permanent injury visible on a skin-covered body part?
        public bool IsRemovableHediff(Hediff hediff) => hediff is Hediff_Injury && hediff.IsPermanent() && hediff.Visible && (hediff.Part?.def.IsSkinCovered(hediff.Part, hediff.pawn.health.hediffSet) ?? false);

        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            // Check all of our hediffs for scars.
            foreach (Hediff hediff in pawn.health.hediffSet.hediffs)
            {
                // Is the hediff a permanent injury visible on a skin-covered body part?
                if (IsRemovableHediff(hediff))
                {
                    // It is, so yield the scarred part.
                    yield return hediff.Part;
                }
            }
            // We're done.
            yield break;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            // Moved this line earlier in code to allow getting its label in the billDoer block.
            Hediff hediff = pawn.health.hediffSet.hediffs.Find(x => x.Part == part && IsRemovableHediff(x));

            if (billDoer != null)
            {
                if (CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer))
                {
                    string text;
                    if (!recipe.successfullyRemovedHediffMessage.NullOrEmpty())
                    {
                        text = string.Format(recipe.successfullyRemovedHediffMessage, billDoer.LabelShort, pawn.LabelShort);
                    }
                    else
                    {
                        // The null check here is a little bit hacky and shouldn't even be necessary, but if it *really* wasn't necessary than the base ApplyOnPawn wouldn't have a null check either... right?
                        text = "MessageSuccessfullyRemovedHediff".Translate(billDoer.LabelShort, pawn.LabelShort, (hediff?.LabelBase ?? part.LabelShort).Named("HEDIFF"), billDoer.Named("SURGEON"), pawn.Named("PATIENT"));
                    }
                    Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent, true);
                }
            }

            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);
            }
        }
    }
}
