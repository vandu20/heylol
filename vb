package com.db.fusion.rs.model;

import org.junit.jupiter.api.Test;
import static org.junit.jupiter.api.Assertions.*;
import java.util.List;
import java.util.Set;

class RestrictedSecurityTest {

    @Test
    void testHasIncorrectRICs_WhenEmpty() {
        RestrictedSecurity security = new RestrictedSecurity();
        assertFalse(security.hasIncorrectRICs(), "Expected no incorrect RICs initially");
    }

    @Test
    void testHasIncorrectRICs_WhenNotEmpty() {
        RestrictedSecurity security = new RestrictedSecurity();
        security.getIncorrectRICs().add("RIC123");
        assertTrue(security.hasIncorrectRICs(), "Expected incorrect RICs to be present");
    }

    @Test
    void testGetCorrectRICs() {
        RestrictedSecurity security = new RestrictedSecurity();
        security.getCorrectRICs().add("CORRECT_RIC_1");
        security.getCorrectRICs().add("CORRECT_RIC_2");

        List<String> correctRICs = security.getCorrectRICs();
        assertEquals(2, correctRICs.size(), "Expected 2 correct RICs");
        assertTrue(correctRICs.contains("CORRECT_RIC_1"));
        assertTrue(correctRICs.contains("CORRECT_RIC_2"));
    }

    @Test
    void testSetAndGetSecurityDescription() {
        RestrictedSecurity security = new RestrictedSecurity();
        security.setSecurityDescription("Test Security");
        assertEquals("Test Security", security.getSecurityDescription());
    }
}
