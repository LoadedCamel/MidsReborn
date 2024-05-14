using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Mids_Reborn.Core;
using Mids_Reborn.Core.Base.Master_Classes;
using Mids_Reborn.Core.BuildFile;
using Mids_Reborn.Core.ShareSystem;
using Mids_Reborn.Core.ShareSystem.RestModels;

namespace Mids_Reborn.Forms.ImportExportItems
{
    internal static class ShareGenerator
    {
        private static BuildManager BuildManager => BuildManager.Instance;

        internal static string GeneratedBuildDataChunk()
        {
            var buildData = BuildManager.GetDataChunk();
            return buildData;
        }

        internal static Image SharedBuildImage(bool useAltGfx = false)
        {
            var gfxBytes = InfoGraphic.Generate(useAltGfx);
            using var stream = new MemoryStream(gfxBytes);
            var image = Image.FromStream(stream);
            return image;
        }

        internal static async Task<OperationResult<TransactionResult>> SubmitOrUpdateBuild(string? id = null)
        {
            OperationResult<TransactionResult> response;

            var dto = new BuildRecordDto
            {
                Name = MidsContext.Character?.Name,
                Archetype = MidsContext.Character?.Archetype?.DisplayName,
                Description = MidsContext.Character?.Comment,
                Primary = MidsContext.Character?.Powersets[0]?.DisplayName,
                Secondary = MidsContext.Character?.Powersets[1]?.DisplayName,
                BuildData = BuildManager.GetShareData(),
                ImageData = InfoGraphic.GenerateImageData()
            };

            if (id is null)
            {
                response = await ShareClient.SubmitBuild(dto);
            }
            else
            {
                response = await ShareClient.UpdateBuild(dto, id);
            }

            return response;
        }
    }
}
