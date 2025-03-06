package com.db.fusion.rs.dao;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

@ExtendWith(MockitoExtension.class)
class RSDaoImplTest {

    @Mock
    private Connection mockConnection;

    @Mock
    private PreparedStatement mockPreparedStatement;

    @Mock
    private ResultSet mockResultSet;

    @InjectMocks
    private RSDaoImpl rsDaoImpl;

    @BeforeEach
    void setUp() throws SQLException {
        // Mocking SQL execution steps
        when(mockConnection.prepareStatement(anyString())).thenReturn(mockPreparedStatement);
        when(mockPreparedStatement.executeQuery()).thenReturn(mockResultSet);
    }

    @Test
    void testFetchRestrictionType_Found() throws SQLException {
        // Mocking ResultSet behavior to return a restriction type
        when(mockResultSet.next()).thenReturn(true);
        when(mockResultSet.getString(1)).thenReturn("RESTRICTED_TYPE_ABC");

        String restrictionType = rsDaoImpl.fetchRestrictionType(mockConnection);

        // Assertions
        assertNotNull(restrictionType);
        assertEquals("RESTRICTED_TYPE_ABC", restrictionType);

        // Verify interactions with mocks
        verify(mockPreparedStatement).setString(1, "21");
        verify(mockPreparedStatement).setString(2, "A");
        verify(mockPreparedStatement).executeQuery();
        verify(mockResultSet, times(1)).next();
        verify(mockResultSet, times(1)).getString(1);
    }

    @Test
    void testFetchRestrictionType_NotFound() throws SQLException {
        // Mocking ResultSet behavior to return no data
        when(mockResultSet.next()).thenReturn(false);

        String restrictionType = rsDaoImpl.fetchRestrictionType(mockConnection);

        // Assertions
        assertNotNull(restrictionType);
        assertEquals("UNKNOWN", restrictionType); // Assuming UNKNOWN is the default fallback

        // Verify interactions with mocks
        verify(mockPreparedStatement).setString(1, "21");
        verify(mockPreparedStatement).setString(2, "A");
        verify(mockPreparedStatement).executeQuery();
        verify(mockResultSet, times(1)).next();
        verify(mockResultSet, never()).getString(1); // Should not fetch data if no row exists
    }
}
