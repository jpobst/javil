namespace Javil;

public class ResolutionException : Exception
{
    public MemberReference MemberReference { get; }


    public ResolutionException (MemberReference member) : base ("Failed to resolve " + member.FullName)
    {
        MemberReference = member;
    }

    public ResolutionException (MemberReference member, Exception innerException) : base ("Failed to resolve " + member.FullName, innerException)
    {
        MemberReference = member;
    }

}
