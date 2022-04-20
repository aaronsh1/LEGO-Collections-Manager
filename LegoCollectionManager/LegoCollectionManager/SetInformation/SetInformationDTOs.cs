using LegoCollectionManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LegoCollectionManager.SetInformation
{
    public class SetInformationDTO
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Headline { get; init; }
        public string OneLiner { get; init; }
        public string[] BulletPoints { get; init; }
        public int PieceCount { get; init; }
        public BuildingInstructions[] Instructions { get; init; }
        public LegoSetImage PrimaryImage { get; init; }
        public LegoSetImage BoxImage { get; init; }
        public LegoSetImage[] Images { get; init; }
        public LegoSetImage[] AdditionalImages { get; init; }

        public int? SetCategory { get; set; }

        public SetCategory SetCategoryNavigation { get; set; }
        public ICollection<SetPieceCategory> SetPieceCategories { get; set; }
        public ICollection<SetPiece> SetPieces { get; set; }
        public ICollection<UserSet> UserSets { get; set; }

        private SetInformationDTO() { }

        public static SetInformationDTO GetDTO(Set setModel, SetInformation setInformation) {
            SetInformationDTO res = new SetInformationDTO() { 
                AdditionalImages = setInformation?.AdditionalImages,
                BoxImage = setInformation?.BoxImage,
                BulletPoints = setInformation?.BulletPoints,
                Description = setInformation?.Description,
                Headline = setInformation?.Headline,
                Images = setInformation?.Images,
                Instructions = setInformation?.Instructions,
                OneLiner = setInformation?.OneLiner,
                PrimaryImage = setInformation?.PrimaryImage,
                PieceCount = setModel.PiecesAmount ?? setInformation?.PieceCount ?? 0,
                SetCategoryNavigation = setModel.SetCategoryNavigation,
                SetPieceCategories = setModel.SetPieceCategories,
                SetPieces = setModel.SetPieces,
                UserSets = setModel.UserSets,
                SetCategory = setModel.SetCategory,
                Name = setModel.SetName,
                Id = setModel.SetId.ToString(),                
            };

            return res;
        }
    }

    public class SetInformation
    {
        public string Id { get; init; }
        public string Name { get; init; }
        public string Description { get; init; }
        public string Headline { get; init; }
        public string OneLiner { get; init; }
        public string[] BulletPoints { get; init; }
        public int PieceCount { get; init; }
        public BuildingInstructions[] Instructions { get; init; }
        public LegoSetImage PrimaryImage { get; init; }
        public LegoSetImage BoxImage { get; init; }
        public LegoSetImage[] Images { get; init; }
        public LegoSetImage[] AdditionalImages { get; init; }
    }

    public class BuildingInstructions
    {
        public string Id { get; init; }
        public int SequenceNumber { get; init; }
        public int SequenceTotal { get; init; }
        public string ImageUrl { get; init; }
        public string FileUrl { get; init; }
    }

    public class LegoSetImage
    {
        public string Uid { get; init; }
        public string FileName { get; init; }
        public string Url { get; init; }
        public string ImageType { get; init; } //TODO: Create an enum
    }
}
