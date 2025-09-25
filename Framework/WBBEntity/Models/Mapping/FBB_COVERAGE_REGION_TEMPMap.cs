using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WBBEntity.Models.Mapping
{
    public class FBB_COVERAGE_REGION_TEMPMap : EntityTypeConfiguration<FBB_COVERAGE_REGION_TEMP>
    {
        public FBB_COVERAGE_REGION_TEMPMap()
        {
            this.HasKey(t => new
            {
                t.FTTX_ID,
            });

//            FTTX_ID	N	NUMBER	N			
//GROUP_AMPHUR	N	VARCHAR2(50)	Y			
//OWNER_PRODUCT	N	VARCHAR2(20)	Y			
//OWNER_TYPE	N	VARCHAR2(20)	Y			
//TOWER_TH	N	VARCHAR2(250)	Y			
//TOWER_EN	N	VARCHAR2(250)	Y			
//ONTARGET_DATE	N	DATE	Y			
//ACTIVEFLAG	N	VARCHAR2(1)	Y	'Y'		
//CREATED_BY	N	VARCHAR2(50)	N			
//CREATED_DATE	N	DATE	N			
//UPDATED_BY	N	VARCHAR2(50)	Y			
//UPDATED_DATE	N	DATE	Y			


            this.Property(t => t.GROUP_AMPHUR)
                .HasMaxLength(50);

            this.Property(t => t.OWNER_PRODUCT)
                .HasMaxLength(20);

            this.Property(t => t.OWNER_TYPE)
                .HasMaxLength(20);

    

            this.Property(t => t.TOWER_TH)
                .HasMaxLength(250);
         
            this.Property(t => t.TOWER_EN)
              .HasMaxLength(250);

            this.Property(t => t.ONTARGET_DATE)
                .IsRequired();
                    
            this.Property(t => t.UPDATED_BY)
                .HasMaxLength(50);

            this.Property(t => t.ACTIVEFLAG)
                .HasMaxLength(1);

            this.Property(t => t.CREATED_BY)
                .IsRequired()
                .HasMaxLength(50);

            this.Property(t => t.CREATED_DATE)
                .IsRequired();

         
            this.Property(t => t.UPDATED_DATE)
                 .IsRequired();


            this.ToTable("FBB_COVERAGE_REGION_TEMP", "WBB");

            this.Property(t => t.GROUP_AMPHUR).HasColumnName("GROUP_AMPHUR");
            this.Property(t => t.OWNER_PRODUCT).HasColumnName("OWNER_PRODUCT");
            this.Property(t => t.OWNER_TYPE).HasColumnName("OWNER_TYPE");
            this.Property(t => t.TOWER_TH).HasColumnName("TOWER_TH");
            this.Property(t => t.TOWER_EN).HasColumnName("TOWER_EN");
            this.Property(t => t.ONTARGET_DATE).HasColumnName("ONTARGET_DATE");
            this.Property(t => t.UPDATED_BY).HasColumnName("UPDATED_BY");
            this.Property(t => t.ACTIVEFLAG).HasColumnName("ACTIVEFLAG");
            this.Property(t => t.CREATED_BY).HasColumnName("CREATED_BY");

            this.Property(t => t.CREATED_DATE).HasColumnName("CREATED_DATE");
            this.Property(t => t.UPDATED_DATE).HasColumnName("UPDATED_DATE");
            this.Property(t => t.FTTX_ID).HasColumnName("FTTX_ID");
         
           
        }
    }
}
