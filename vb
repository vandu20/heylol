import static org.mockito.Mockito.*;
import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.Mockito;
import java.sql.ResultSet;
import java.sql.SQLException;
import java.util.Arrays;
import java.util.List;

public class RestrictedSecurityTest {

    private ResultSet mockResultSet;

    @BeforeEach
    void setup() throws SQLException {
        // Mocking ResultSet
        mockResultSet = mock(ResultSet.class);

        // Mock behavior for ResultSet
        when(mockResultSet.getString(2)).thenReturn("Test Security"); // SECURITY_NAME
        when(mockResultSet.getString(3)).thenReturn("ISIN"); // INSTRUMENT_ID_TYPE
        when(mockResultSet.getString(4)).thenReturn("US1234567890"); // INSTRUMENT_ID_VALUE
    }

    @Test
    void testRestrictedSecurityInitialization() throws SQLException {
        // Creating RestrictedSecurity object with mocked ResultSet
        RestrictedSecurity restrictedSecurity = new RestrictedSecurity(mockResultSet);

        // Verifying the security details
        assertNotNull(restrictedSecurity);
        assertEquals(1, restrictedSecurity.getISINS().size());
        assertEquals("US1234567890", restrictedSecurity.getISINS().get(0));
        assertEquals("Test Security", restrictedSecurity.getSecurityDescription());
    }

    @Test
    void testListOfRestrictedSecurities() throws SQLException {
        // Creating multiple mock RestrictedSecurity objects
        RestrictedSecurity security1 = new RestrictedSecurity(mockResultSet);
        RestrictedSecurity security2 = new RestrictedSecurity(mockResultSet);

        List<RestrictedSecurity> mockSecurities = Arrays.asList(security1, security2);

        // Checking the list
        assertNotNull(mockSecurities);
        assertEquals(2, mockSecurities.size());
    }
}
