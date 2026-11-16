using Enums;

namespace Models;

public class DowntimeEvents
{
    public int id { get; set; }

    public int project_id { get; set; }

    public DateTime started_at { get; set; }

    public DateTime? resolved_at { get; set; }

    public DowntimeStatus status { get; set; } = DowntimeStatus.Ongoing;

    public Projects? Project { get; set; }
}