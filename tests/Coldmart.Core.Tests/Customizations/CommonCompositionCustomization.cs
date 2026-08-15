using AutoFixture;
using AutoFixture.AutoMoq;

namespace Coldmart.Core.Tests.Customizations;

public sealed class CommonCompositionCustomization : CompositeCustomization
{
    public CommonCompositionCustomization()
        : base(
            new DateTimeOffsetCustomization(),
            new IdentityCustomization(),
            new AutoMoqCustomization() { ConfigureMembers = true })
    { }
}
