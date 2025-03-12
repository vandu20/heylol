package com.db.fusion.rs.model;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;

import java.util.*;

class RestrictedSecurityTest {

    @Test
    void testEquals_SameObject() {
        RestrictedSecurity security1 = new RestrictedSecurity();
        assertEquals(security1, security1, "Expected object to be equal to itself");
    }

    @Test
    void testEquals_DifferentObjectsWithSameData() {
        List<String> correctRICs = Arrays.asList("RIC1", "RIC2");
        Set<String> incorrectRICs = new HashSet<>(Collections.singletonList("RIC3"));
        List<String> ISINs = Arrays.asList("ISIN1", "ISIN2");
        List<String> CUSIPs = Arrays.asList("CUSIP1", "CUSIP2");
        List<String> WPKs = Arrays.asList("WPK1", "WPK2");
        String securityDescription = "Test Security";

        RestrictedSecurity security1 = new RestrictedSecurity(correctRICs, incorrectRICs, ISINs, CUSIPs, WPKs, securityDescription);
        RestrictedSecurity security2 = new RestrictedSecurity(correctRICs, incorrectRICs, ISINs, CUSIPs, WPKs, securityDescription);

        assertEquals(security1, security2, "Expected objects with same data to be equal");
    }

    @Test
    void testEquals_DifferentObjectsWithDifferentData() {
        RestrictedSecurity security1 = new RestrictedSecurity();
        RestrictedSecurity security2 = new RestrictedSecurity(Arrays.asList("RIC1"), new HashSet<>(), new ArrayList<>(), new ArrayList<>(), new ArrayList<>(), "Test Security");

        assertNotEquals(security1, security2, "Expected objects with different data to be not equal");
    }

    @Test
    void testHashCode_SameData() {
        List<String> correctRICs = Arrays.asList("RIC1", "RIC2");
        Set<String> incorrectRICs = new HashSet<>(Collections.singletonList("RIC3"));
        List<String> ISINs = Arrays.asList("ISIN1", "ISIN2");
        List<String> CUSIPs = Arrays.asList("CUSIP1", "CUSIP2");
        List<String> WPKs = Arrays.asList("WPK1", "WPK2");
        String securityDescription = "Test Security";

        RestrictedSecurity security1 = new RestrictedSecurity(correctRICs, incorrectRICs, ISINs, CUSIPs, WPKs, securityDescription);
        RestrictedSecurity security2 = new RestrictedSecurity(correctRICs, incorrectRICs, ISINs, CUSIPs, WPKs, securityDescription);

        assertEquals(security1.hashCode(), security2.hashCode(), "Expected objects with same data to have the same hashCode");
    }

    @Test
    void testToString() {
        RestrictedSecurity security = new RestrictedSecurity(Arrays.asList("RIC1"), new HashSet<>(Collections.singletonList("RIC3")), Arrays.asList("ISIN1"), Arrays.asList("CUSIP1"), Arrays.asList("WPK1"), "Test Security");

        String expectedString = "RestrictedSecurity(correctRICs=[RIC1], incorrectRICs=[RIC3], ISINs=[ISIN1], CUSIPs=[CUSIP1], WPKs=[WPK1], securityDescription=Test Security)";
        assertEquals(expectedString, security.toString(), "Expected toString output does not match");
    }

    @Test
    void testSetCorrectRICs() {
        RestrictedSecurity security = new RestrictedSecurity();
        List<String> newRICs = Arrays.asList("RIC1", "RIC2");
        security.setCorrectRICs(newRICs);
        assertEquals(newRICs, security.getCorrectRICs(), "Expected correctRICs to be set properly");
    }

    @Test
    void testSetIncorrectRICs() {
        RestrictedSecurity security = new RestrictedSecurity();
        Set<String> newIncorrectRICs = new HashSet<>(Collections.singletonList("RIC3"));
        security.setIncorrectRICs(newIncorrectRICs);
        assertEquals(newIncorrectRICs, security.getIncorrectRICs(), "Expected incorrectRICs to be set properly");
    }

    @Test
    void testSetISINs() {
        RestrictedSecurity security = new RestrictedSecurity();
        List<String> newISINs = Arrays.asList("ISIN1", "ISIN2");
        security.setISINs(newISINs);
        assertEquals(newISINs, security.getISINs(), "Expected ISINs to be set properly");
    }

    @Test
    void testSetCUSIPs() {
        RestrictedSecurity security = new RestrictedSecurity();
        List<String> newCUSIPs = Arrays.asList("CUSIP1", "CUSIP2");
        security.setCUSIPs(newCUSIPs);
        assertEquals(newCUSIPs, security.getCUSIPs(), "Expected CUSIPs to be set properly");
    }

    @Test
    void testSetWPKs() {
        RestrictedSecurity security = new RestrictedSecurity();
        List<String> newWPKs = Arrays.asList("WPK1", "WPK2");
        security.setWPKs(newWPKs);
        assertEquals(newWPKs, security.getWPKs(), "Expected WPKs to be set properly");
    }

    @Test
    void testCanEqual() {
        RestrictedSecurity security1 = new RestrictedSecurity();
        RestrictedSecurity security2 = new RestrictedSecurity();
        assertTrue(security1.canEqual(security2), "Expected canEqual to return true for objects of the same type");
    }
}
