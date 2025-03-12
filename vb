package com.db.fusion.rs.dao;

import com.db.fusion.rs.model.RestrictedSecurity;
import com.db.fusion.rs.service.RestrictedSecurityService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.*;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class RSDaoImplTest {

    @InjectMocks
    private RSDaoImpl rsDao;

    @Mock
    private RestrictedSecurityService restrictedSecurityService;

    @Mock
    private Connection mockConnection;

    @Mock
    private PreparedStatement mockPreparedStatement;

    @Mock
    private ResultSet mockResultSet;

    @BeforeEach
    void setUp() throws SQLException {
        when(mockConnection.prepareStatement(anyString())).thenReturn(mockPreparedStatement);
        when(mockPreparedStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockPreparedStatement.setFetchSize(anyInt())).thenReturn(mockPreparedStatement);
    }

    @Test
    void testFetchRestrictions_Success() throws SQLException {
        // Mocking ResultSet behavior
        when(mockResultSet.next()).thenReturn(true, true, false); // Simulate two rows

        when(mockResultSet.getString(1)).thenReturn("ID_1", "ID_2"); // Mocking ID retrieval
        when(mockResultSet.getString(2)).thenReturn("Security A", "Security B");

        Set<String> incorrectRICs = new HashSet<>();
        List<RestrictedSecurity> result = rsDao.fetchRestrictions(mockConnection, incorrectRICs);

        assertNotNull(result, "Result should not be null");
        assertEquals(2, result.size(), "Expected two restricted securities");
        verify(mockPreparedStatement, times(3)).setString(anyInt(), anyString()); // Ensure setString is called
        verify(mockPreparedStatement).executeQuery(); // Ensure query is executed
    }

    @Test
    void testFetchRestrictions_EmptyResultSet() throws SQLException {
        when(mockResultSet.next()).thenReturn(false); // Simulate no data

        Set<String> incorrectRICs = new HashSet<>();
        List<RestrictedSecurity> result = rsDao.fetchRestrictions(mockConnection, incorrectRICs);

        assertNotNull(result, "Result should not be null");
        assertTrue(result.isEmpty(), "Expected empty list when no data found");
        verify(mockPreparedStatement).executeQuery(); // Ensure query is executed
    }

    @Test
    void testFetchRestrictions_SQLException() throws SQLException {
        when(mockConnection.prepareStatement(anyString())).thenThrow(new SQLException("Database Error"));

        Set<String> incorrectRICs = new HashSet<>();
        assertThrows(SQLException.class, () -> rsDao.fetchRestrictions(mockConnection, incorrectRICs), "Expected SQLException to be thrown");
    }
}
