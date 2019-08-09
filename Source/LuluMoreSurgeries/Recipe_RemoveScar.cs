using RimWorld;
using System.Collections.Generic;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.MoreSurgeries
{
    public class Recipe_RemoveScar : Recipe_RemoveHediff
    {
        // Is the hediff a permanent injury visible on a skin-covered body part?
        public bool IsRemovableHediff(Hediff diff) => diff is Hediff_Injury && diff.IsPermanent() && diff.Visible && (diff.Part?.def.IsSkinCovered(diff.Part, diff.pawn.health.hediffSet) ?? false);

        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            foreach (Hediff diff in pawn.health.hediffSet.hediffs)
            {
                if (diff.Part != null)
                {
                    if (IsRemovableHediff(diff))
                    {
                        yield return diff.Part;
                    }
                }
            }
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
                        text = "MessageSuccessfullyRemovedHediff".Translate(billDoer.LabelShort, pawn.LabelShort, hediff.LabelBase.Named("HEDIFF"), billDoer.Named("SURGEON"), pawn.Named("PATIENT"));
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
