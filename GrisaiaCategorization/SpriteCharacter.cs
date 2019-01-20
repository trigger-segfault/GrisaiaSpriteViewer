using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Grisaia;
using Grisaia.Asmodean;

namespace Grisaia {
	public class SpriteCharacter : IKey<string> {
		public string Id { get; set; }
		public int Index { get; set; }
		public Dictionary<SpriteLighting, SpriteCharacterLighting> Lightings { get; }
			= new Dictionary<SpriteLighting, SpriteCharacterLighting>();
		public List<SpriteCharacterLighting> SortedLightings { get; }
			= new List<SpriteCharacterLighting>();
		/*public SpriteCharacterPose Get(SpriteLighting lighting, SpriteHeight height, int pose) {
			if (!Lightings.TryGetValue(lighting, out var l)) l = Lightings.First().Value;
			if (!l.Heights.TryGetValue(height, out var h)) h = l.Heights.First().Value;
			if (!h.Poses.TryGetValue(pose, out var p)) p = h.Poses.First().Value;
			return p;
		}*/
	}
	public class SpriteCharacterLighting : IKey<SpriteLighting> {
		public SpriteLighting Id { get; set; }
		public int Index { get; set; }
		public Dictionary<SpriteDistance, SpriteCharacterDistance> Distances { get; }
			= new Dictionary<SpriteDistance, SpriteCharacterDistance>();
		public List<SpriteCharacterDistance> SortedDistances { get; }
			= new List<SpriteCharacterDistance>();
	}
	public class SpriteCharacterDistance : IKey<SpriteDistance> {
		public SpriteDistance Id { get; set; }
		public int Index { get; set; }
		/*public Dictionary<SpriteSize, SpriteCharacterSize> Sizes { get; }
			= new Dictionary<SpriteSize, SpriteCharacterSize>();
		public List<SpriteCharacterSize> SortedSizes { get; }
			= new List<SpriteCharacterSize>();*/
		public Dictionary<int, SpriteCharacterPose> Poses { get; }
			= new Dictionary<int, SpriteCharacterPose>();
		public List<SpriteCharacterPose> SortedPoses { get; }
			= new List<SpriteCharacterPose>();
	}
	/*public class SpriteCharacterSize : IKey<SpriteSize> {
		public SpriteSize Id { get; set; }
		public int Index { get; set; }
		public Dictionary<int, SpriteCharacterPose> Poses { get; }
			= new Dictionary<int, SpriteCharacterPose>();
		public List<SpriteCharacterPose> SortedPoses { get; }
			= new List<SpriteCharacterPose>();
	}*/
	public class SpriteCharacterPose : IKey<int> {
		public int Id { get; set; }
		public int Index { get; set; }
		public Dictionary<SpriteBlush, SpriteCharacterBlush> Blushes { get; }
			= new Dictionary<SpriteBlush, SpriteCharacterBlush>();
		public List<SpriteCharacterBlush> SortedBlushes { get; }
			= new List<SpriteCharacterBlush>();

		//public List<SpriteNyanmel> SortedNyanmels { get; } = new List<SpriteNyanmel>();
	}
	public class SpriteCharacterBlush : IKey<SpriteBlush> {
		public SpriteBlush Id { get; set; }
		public int Index { get; set; }
		public Dictionary<int, SpritePartList> PartTypes { get; } = new Dictionary<int, SpritePartList>();
		public List<SpritePartList> SortedPartTypes { get; } = new List<SpritePartList>();

		public bool TryGetPartTypes(int[] typeIds, int partId, out SpritePart[] parts) {
			parts = new SpritePart[typeIds.Length];
			bool found = false;
			for (int groupIndex = 0; groupIndex < typeIds.Length; groupIndex++) {
				if (PartTypes.TryGetValue(typeIds[groupIndex], out var ptypes) && ptypes.Parts.TryGetValue(partId, out parts[groupIndex]))
					found = true;
			}
			return found;
		}
		public bool TryGetFirstPartTypes(int[] typeIds, out int partId, out SpritePart[] parts) {
			parts = new SpritePart[typeIds.Length];
			bool found = false;
			partId = int.MaxValue;
			for (int groupIndex = 0; groupIndex < typeIds.Length; groupIndex++) {
				int typeId = typeIds[groupIndex];
				if (PartTypes.TryGetValue(typeId, out var ptypes) && ptypes.SortedParts.Any()) {
					parts[groupIndex] = ptypes.SortedParts.First();
					partId = Math.Min(parts[groupIndex].Id, partId);
					found = true;
				}
			}
			if (found) {
				for (int groupIndex = 0; groupIndex < typeIds.Length; groupIndex++) {
					if (parts[groupIndex] != null && parts[groupIndex].Id != partId)
						parts[groupIndex] = null;
				}
			}
			else {
				partId = -1;
			}
			return found;
		}

		//public List<SpriteNyanmel> SortedNyanmels { get; } = new List<SpriteNyanmel>();
	}
	public class SpritePartList : IKey<int> {
		public int Id { get; set; }
		public int Index { get; set; }
		public Dictionary<int, SpritePart> Parts { get; } = new Dictionary<int, SpritePart>();
		public List<SpritePart> SortedParts { get; } = new List<SpritePart>();
	}
	public class SpritePart : IKey<int> {
		public int Id { get; set; }
		public int Index { get; set; }
		public string FileName { get; set; }
		public string PngFile => Path.ChangeExtension(FileName, ".png");
		public string JsonFile => Path.ChangeExtension(FileName, ".json");

		public Hg3Image Cached { get; set; }

		public override string ToString() => FileName;
	}
}
