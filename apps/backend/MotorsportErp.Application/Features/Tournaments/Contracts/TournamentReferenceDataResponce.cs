using MotorsportErp.Application.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MotorsportErp.Application.Features.Tournaments.Contracts;

public class TournamentReferenceDataResponce
{
    public int MinRacesToBecomeOrganizer { get; set; }

    public List<EnumValueResponse> TournamentStatuses { get; set; } = [];
    public List<EnumValueResponse> TournamentApplicationStatuses { get; set; } = [];
}
