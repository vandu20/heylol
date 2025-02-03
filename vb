package com.example.quorumdb;

import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.jdbc.core.JdbcTemplate;
import org.springframework.jdbc.datasource.DriverManagerDataSource;
import org.springframework.context.annotation.Bean;

import java.util.List; 
import java.util.Map;

@SpringBootApplication
public class QuorumDbApplication {

    public static void main(String[] args) {
        SpringApplication.run(QuorumDbApplication.class, args);
    }

    @Bean
    public JdbcTemplate jdbcTemplate() {
        DriverManagerDataSource dataSource = new DriverManagerDataSource();
        dataSource.setDriverClassName("oracle.jdbc.OracleDriver");
        dataSource.setUrl("jdbc:oracle:thin:@your_quorum_db_host:port:SID");
        dataSource.setUsername("your_db_username");
        dataSource.setPassword("your_db_password");

        return new JdbcTemplate(dataSource);
    }

    @Bean
    public CommandLineRunner run(JdbcTemplate jdbcTemplate) {
        return args -> {
            // SQL Queries
            String sqlRestrictionType = "SELECT DISTINCT v." + ExtractorConstants.RESTRICTION_TYPE_NAME +
                    " FROM " + ExtractorConstants.VW_RESTRICTION + " v WHERE v." + ExtractorConstants.RESTRICTION_TYPE_CODE +
                    " = ? AND ROWNUM <= 1 AND " + ExtractorConstants.STATUS_CODE + " = ?";

            String sqlRestrictions = "SELECT DISTINCT v." + ExtractorConstants.RESTRICTION_ID + ", v." + ExtractorConstants.SECURITY_NAME +
                    ", v." + ExtractorConstants.INSTRUMENT_ID_TYPE + ", (CASE WHEN v." + ExtractorConstants.INSTRUMENT_ID_TYPE +
                    " = ? THEN substr(v." + ExtractorConstants.INSTRUMENT_ID_VALUE + ", 1, " + ExtractorConstants.RIC_MAX_SIZE + ") ELSE v." +
                    ExtractorConstants.INSTRUMENT_ID_VALUE + " END) AS " + ExtractorConstants.INSTRUMENT_ID_VALUE +
                    " FROM (SELECT * FROM (SELECT DISTINCT t." + ExtractorConstants.INSTRUMENT_ISIN + " AS " +
                    ExtractorConstants.RESTRICTION_ID + ", t." + ExtractorConstants.INSTRUMENT_ISIN + ", " +
                    ExtractorConstants.INSTRUMENT_CUSIP + ", t." + ExtractorConstants.INSTRUMENT_WKN + ", " +
                    ExtractorConstants.INSTRUMENT_RIC + ", t." + ExtractorConstants.SECURITY_NAME +
                    " FROM " + ExtractorConstants.VW_RESTRICTION + " t WHERE t." + ExtractorConstants.STATUS_CODE +
                    " = ? AND t." + ExtractorConstants.INSTRUMENT_ISIN + " IS NOT NULL AND t." +
                    ExtractorConstants.RESTRICTION_TYPE_CODE + " = ?)) v ORDER BY " + ExtractorConstants.RESTRICTION_ID;

            // Execute queries
            List<Map<String, Object>> restrictionTypeResult = jdbcTemplate.queryForList(sqlRestrictionType, "yourRestrictionCode", "yourStatusCode");
            List<Map<String, Object>> restrictionsResult = jdbcTemplate.queryForList(sqlRestrictions, "yourInstrumentType", "yourStatusCode", "yourRestrictionCode");

            // Print results
            System.out.println("Restriction Type Results:");
            for (Map<String, Object> row : restrictionTypeResult) {
                System.out.println(row);
            }

            System.out.println("Restrictions Results:");
            for (Map<String, Object> row : restrictionsResult) {
                System.out.println(row);
            }
        };
    }
}
