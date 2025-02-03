package com.example.quorumdb;

import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.stereotype.Repository;
import org.springframework.data.jpa.repository.Query;

import javax.persistence.Entity;
import javax.persistence.Id;
import java.util.List;

// Main application class
@SpringBootApplication
public class QuorumDbApplication {

    // Define entity to map to database
    @Entity
    public static class RestrictionType {

        @Id
        private String restrictionId;
        private String securityName;
        private String instrumentIdType;
        private String instrumentIdValue;

        // Getters and Setters
        public String getRestrictionId() {
            return restrictionId;
        }

        public void setRestrictionId(String restrictionId) {
            this.restrictionId = restrictionId;
        }

        public String getSecurityName() {
            return securityName;
        }

        public void setSecurityName(String securityName) {
            this.securityName = securityName;
        }

        public String getInstrumentIdType() {
            return instrumentIdType;
        }

        public void setInstrumentIdType(String instrumentIdType) {
            this.instrumentIdType = instrumentIdType;
        }

        public String getInstrumentIdValue() {
            return instrumentIdValue;
        }

        public void setInstrumentIdValue(String instrumentIdValue) {
            this.instrumentIdValue = instrumentIdValue;
        }

        @Override
        public String toString() {
            return "RestrictionType{" +
                    "restrictionId='" + restrictionId + '\'' +
                    ", securityName='" + securityName + '\'' +
                    ", instrumentIdType='" + instrumentIdType + '\'' +
                    ", instrumentIdValue='" + instrumentIdValue + '\'' +
                    '}';
        }
    }

    // Repository interface for handling database queries
    @Repository
    public interface RestrictionTypeRepository extends JpaRepository<RestrictionType, String> {

        @Query("SELECT r FROM RestrictionType r WHERE r.restrictionId = ?1 AND r.securityName = ?2")
        List<RestrictionType> findByRestrictionTypeCodeAndStatusCode(String restrictionTypeCode, String statusCode);

        // Custom query for the first SQL query
        @Query("SELECT DISTINCT v.restrictionTypeName FROM RestrictionType v WHERE v.restrictionTypeCode = ?1 AND v.statusCode = ?2")
        List<String> findRestrictionTypeByCodeAndStatus(String restrictionTypeCode, String statusCode);

        // Custom query for the second SQL query
        @Query("SELECT DISTINCT v.restrictionId, v.securityName, v.instrumentIdType, " +
                "CASE WHEN v.instrumentIdType = ?1 THEN SUBSTRING(v.instrumentIdValue, 1, ?2) ELSE v.instrumentIdValue END " +
                "FROM RestrictionType v WHERE v.statusCode = ?3 AND v.restrictionTypeCode = ?4")
        List<Object[]> findRestrictionsByTypeAndStatus(String instrumentType, int ricMaxSize, String statusCode, String restrictionTypeCode);
    }

    @Autowired
    private RestrictionTypeRepository restrictionTypeRepository;

    public static void main(String[] args) {
        SpringApplication.run(QuorumDbApplication.class, args);
    }

    // CommandLineRunner to execute database operations after application starts
    @Bean
    public CommandLineRunner run() {
        return args -> {
            // Execute the first query to find distinct restriction type names
            List<String> restrictionTypes = restrictionTypeRepository.findRestrictionTypeByCodeAndStatus("yourRestrictionCode", "yourStatusCode");
            
            System.out.println("Restriction Type Results:");
            restrictionTypes.forEach(System.out::println);

            // Execute the second query to find restrictions with their details
            List<Object[]> restrictions = restrictionTypeRepository.findRestrictionsByTypeAndStatus(
                    "yourInstrumentType", ExtractorConstants.RIC_MAX_SIZE, "yourStatusCode", "yourRestrictionCode");

            System.out.println("Restrictions Results:");
            for (Object[] row : restrictions) {
                System.out.println("Restriction ID: " + row[0] + ", Security Name: " + row[1] + 
                        ", Instrument ID Type: " + row[2] + ", Instrument ID Value: " + row[3]);
            }
        };
    }
}
