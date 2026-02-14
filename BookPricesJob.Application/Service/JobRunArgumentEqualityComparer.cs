using BookPricesJob.Common.Domain;

namespace BookPricesJob.Application.Service;

public class JobRunArgumentEqualityComparer : IEqualityComparer<JobRunArgument>
{
    public bool Equals(JobRunArgument? first, JobRunArgument? second)
    {
        if (first is null || second is null)
            return false;

        return first.Name == second.Name
               && first.Type == second.Type
               && first.Values.SequenceEqual(second.Values);
    }

    public int GetHashCode(JobRunArgument obj)
    {
        var hash = new HashCode();
        hash.Add(obj.Name);
        hash.Add(obj.Type);
        foreach (var value in obj.Values)
            hash.Add(value);

        return hash.ToHashCode();
    }
}