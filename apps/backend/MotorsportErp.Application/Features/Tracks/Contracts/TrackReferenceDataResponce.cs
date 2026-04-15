using MotorsportErp.Application.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorsportErp.Application.Features.Tracks.Contracts;
public  class TrackReferenceDataResponce
{
    public int MinRacesToCreateTrack { get; set; }
    public int DefaultTrackConfirmationThreshold { get; set; }

    public List<EnumValueResponse> TrackStatuses { get; set; } = [];
}
