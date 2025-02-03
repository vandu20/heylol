package com.example.quoromtest;

import org.springframework.boot.CommandLineRunner;
import org.springframework.boot.SpringApplication;
import org.springframework.boot.autoconfigure.SpringBootApplication;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.jdbc.core.JdbcTemplate;
import java.util.List;
import java.util.Map;

@SpringBootApplication
public class QuoromTest implements CommandLineRunner {

    @Autowired
    private JdbcTemplate jdbcTemplate;

    public static void main(String[] args) {
        SpringApplication.run(QuoromTest.class, args);
    }

    @Override
    public void run(String... args) {
        // Fetch restriction types
        System.out.println("Fetching restriction types...");
        List<Map<String, Object>> restrictionTypes = getRestrictionTypes("ABC", "ACTIVE");
        restrictionTypes.forEach(row -> System.out.println(row));

        // Fetch restrictions
        System.out.println("\nFetching restrictions...");
        List<Map<String, Object>> restrictions = getRestrictions("ISIN", "ACTIVE", "ABC");
        restrictions.forEach(row -> System.out.println(row));
    }

    // Query to fetch restriction types
    private List<Map<String, Object>> getRestrictionTypes(String restrictionTypeCode, String statusCode) {
        String sql = "SELECT DISTINCT v.RESTRICTION_TYPE_NAME " +
                     "FROM VW_RESTRICTION v " +
                     "WHERE v.RESTRICTION_TYPE_CODE = ? AND ROWNUM <= 1 AND v.STATUS_CODE = ?";
        return jdbcTemplate.queryForList(sql, restrictionTypeCode, statusCode);
    }

    // Query to fetch restrictions
    private List<Map<String, Object>> getRestrictions(String instrumentType, String statusCode, String restrictionTypeCode) {
        String sql = "SELECT DISTINCT v.RESTRICTION_ID, v.SECURITY_NAME, v.INSTRUMENT_ID_TYPE, " +
                     "CASE WHEN v.INSTRUMENT_ID_TYPE = ? THEN SUBSTR(v.INSTRUMENT_ID_VALUE, 1, 10) ELSE v.INSTRUMENT_ID_VALUE END AS INSTRUMENT_ID_VALUE " +
                     "FROM VW_RESTRICTION v " +
                     "WHERE v.STATUS_CODE = ? AND v.RESTRICTION_TYPE_CODE = ? " +
                     "ORDER BY v.RESTRICTION_ID";
        return jdbcTemplate.queryForList(sql, instrumentType, statusCode, restrictionTypeCode);
    }
}
