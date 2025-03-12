package com.db.fusion.rs.dao.impl;

import com.db.fusion.rs.model.RestrictedSecurity;
import com.db.fusion.rs.service.RestrictedSecurityService;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;

import java.sql.*;
import java.util.*;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertFalse;
import static org.mockito.ArgumentMatchers.*;
import static org.mockito.Mockito.*;

@ExtendWith(MockitoExtension.class)
class RSDaoImplTest {

    @Mock
    private Connection mockConnection;

    @Mock
    private PreparedStatement mockPreparedStatement;

    @Mock
    private ResultSet mockResultSet;

    @Mock
    private RestrictedSecurityService restrictedSecurityService;

    @InjectMocks
    private RSDaoImpl rsDaoImpl;

    private static final String SQL_RESTRICTIONS = "SELECT DISTINCT v.RESTRICTION_ID, v.SECURITY_NAME FROM VW_RESTRICTION v WHERE v.STATUS_CODE = ? AND v.RESTRICTION_TYPE_CODE = ?";

    @BeforeEach
    void setUp() throws SQLException {
        when(mockConnection.prepareStatement(anyString())).thenReturn(mockPreparedStatement);
        when(mockPreparedStatement.executeQuery()).thenReturn(mockResultSet);
        when(mockPreparedStatement.getFetchSize()).thenReturn(1000);

        // Using lenient() to prevent UnnecessaryStubbingException
        lenient().when(mockResultSet.next()).thenReturn(true, true, false);
        lenient().when(mockResultSet.getString("RESTRICTION_ID")).thenReturn("ID_1", "ID_2");
        lenient().when(mockResultSet.getString("SECURITY_NAME")).thenReturn("Security A", "Security B");
    }

    @Test
    void testFetchRestrictions_Success() throws SQLException {
        Set<String> incorrectRICs = new HashSet<>();

        List<RestrictedSecurity> restrictions = rsDaoImpl.fetchRestrictions(mockConnection, incorrectRICs);

        assertEquals(2, restrictions.size());
        assertFalse(restrictions.isEmpty());

        // Verify interactions
        verify(mockConnection, times(1)).prepareStatement(anyString());
        verify(mockPreparedStatement, times(1)).executeQuery();
        verify(mockResultSet, atLeastOnce()).next();
        verify(mockResultSet, atLeastOnce()).getString("RESTRICTION_ID");
        verify(mockResultSet, atLeastOnce()).getString("SECURITY_NAME");

        verifyNoMoreInteractions(mockConnection, mockPreparedStatement, mockResultSet);
    }
}
