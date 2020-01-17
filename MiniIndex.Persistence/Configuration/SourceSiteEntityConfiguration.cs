﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MiniIndex.Models;
using MiniIndex.Models.SourceSites;

namespace MiniIndex.Persistence.Configuration
{
    public class SourceSiteEntityConfiguration : IEntityTypeConfiguration<SourceSite>
    {
        public void Configure(EntityTypeBuilder<SourceSite> builder)
        {
            builder.HasKey(x => x.ID);

            builder.HasDiscriminator(x => x.SiteName)
                .HasValue<ThingiverseSource>("Thingiverse")
                .HasValue<ShapewaysSource>("Shapeways")
                .HasValue<GumroadSource>("Gumroad")
                .HasValue<PatreonSource>("Patreon")
                .HasValue<MyMiniFactorySource>("MyMiniFactory")
                .HasValue<WebsiteSource>("Website");

            builder.Ignore(x => x.BaseUri);
            builder.Ignore(x => x.CreatorPageUri);
        }
    }
}