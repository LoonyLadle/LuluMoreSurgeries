using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

#pragma warning disable IDE1006 // Naming Styles

namespace LoonyLadle.MoreSurgeries
{
	public class Recipe_InstallCosmeticBodyPart : Recipe_InstallArtificialBodyPart
	{
		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			foreach (BodyPartRecord part in MoreSurgeriesHelper.GetPartsPossibleToInstallArtificialPart(pawn, recipe))
			{
				if (part.def.canSuggestAmputation && part.def.beautyRelated)
				{
					foreach (Hediff hediff in pawn.health.hediffSet.hediffs.Where(x => x.Part == part))
					{
						if ((hediff is Hediff_Injury && hediff.IsPermanent()) || hediff is Hediff_MissingPart)
						{
							yield return part;
						}
					}
				}
			}
			yield break;
		}
	}
}
