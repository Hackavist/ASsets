using System;
using Assets.Models.DataModels;

namespace Assets.Models.Dtos
{
    public class RepositionDto
    {
        public string OldPosition { get; set; }
        public string NewPosition { get; set; }

        public RepositionDto(Repositions reposition)
        {
            OldPosition = reposition.OldPosition;
            NewPosition = reposition.NewPosition;
        }
    }
}
