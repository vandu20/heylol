import static org.mockito.Mockito.*;
import static org.junit.jupiter.api.Assertions.*;

import java.sql.Connection;
import java.sql.PreparedStatement;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

@ExtendWith(MockitoExtension.class)
class RSDaoImplTest {

    private static final Logger LOGGER = LoggerFactory.getLogger(RSDaoImplTest.class);

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

    @BeforeEach
    void setUp() throws SQLException {
        // Mocking the behavior of Connection to return a PreparedStatement
        when(mockConnection.prepareStatement(anyString())).thenReturn(mockPreparedStatement);

        // Mocking the execution of a query to return a ResultSet
        when(mockPreparedStatement.executeQuery()).thenReturn(mockResultSet);
    }

    @Test
    void testFetchRestrictionType() throws SQLException {
        // Arrange
        when(mockResultSet.next()).thenReturn(true);
        when(mockResultSet.getString(1)).thenReturn("TEST_TYPE");

        // Act
        String restrictionType = rsDaoImpl.fetchRestrictionType(mockConnection);

        // Assert
        assertNotNull(restrictionType, "Restriction type should not be null");
        assertEquals("TEST_TYPE", restrictionType, "Fetched restriction type should match expected value");

        // Verify interactions
        verify(mockPreparedStatement, times(1)).setString(1, "21");
        verify(mockPreparedStatement, times(1)).setString(2, "A");
        verify(mockPreparedStatement, times(1)).executeQuery();
        verify(mockResultSet, times(1)).next();
        verify(mockResultSet, times(1)).getString(1);
    }

    @Test
    void testFetchRestrictions() throws SQLException {
        // Arrange
        Set<String> incorrectRICs = new HashSet<>();
        when(mockResultSet.next()).thenReturn(true, false);
        when(mockResultSet.getString(1)).thenReturn("ID_123");

        RestrictedSecurity mockSecurity = new RestrictedSecurity();
        when(restrictedSecurityService.process(any(ResultSet.class), any(RestrictedSecurity.class)))
                .thenAnswer(invocation -> {
                    LOGGER.info("Processing restricted security...");
                    return null;
                });

        // Act
        List<RestrictedSecurity> restrictions = rsDaoImpl.fetchRestrictions(mockConnection, incorrectRICs);

        // Assert
        assertNotNull(restrictions, "Restrictions list should not be null");
        assertFalse(restrictions.isEmpty(), "Restrictions list should not be empty");
        assertEquals(1, restrictions.size(), "Expected one restriction entry");

        // Verify interactions
        verify(mockPreparedStatement, times(1)).setString(1, "RIC");
        verify(mockPreparedStatement, times(1)).setString(2, "A");
        verify(mockPreparedStatement, times(1)).setString(3, "21");
        verify(mockResultSet, times(1)).next();
        verify(restrictedSecurityService, times(1)).process(any(ResultSet.class), any(RestrictedSecurity.class));
    }
}
