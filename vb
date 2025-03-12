import static org.mockito.Mockito.*;
import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;
import org.mockito.junit.jupiter.MockitoExtension;

import javax.xml.stream.XMLStreamWriter;
import javax.xml.stream.XMLOutputFactory;
import java.io.ByteArrayOutputStream;
import java.sql.Connection;
import java.sql.SQLException;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.List;
import java.util.Set;

@ExtendWith(MockitoExtension.class)
public class StaXCallbackImplTest {

    @InjectMocks
    private StaXCallbackImpl staXCallbackImpl;

    @Mock
    private RSDao rsDao;

    @Mock
    private Connection connection;

    private XMLStreamWriter xtw;
    private ByteArrayOutputStream output;

    @BeforeEach
    void setup() throws Exception {
        MockitoAnnotations.openMocks(this);
        XMLOutputFactory xof = XMLOutputFactory.newInstance();
        output = new ByteArrayOutputStream(5000000);
        xtw = xof.createXMLStreamWriter(output, "UTF-8");
    }

    /***
     * Test case for successful execution of doInXMLStream
     */
    @Test
    void testDoInXMLStream_Success() throws Exception {
        // Mock return values
        List<RestrictedSecurity> mockRestrictions = new ArrayList<>();
        mockRestrictions.add(new RestrictedSecurity("RIC001"));
        mockRestrictions.add(new RestrictedSecurity("RIC002"));

        when(rsDao.fetchRestrictions(any(), any())).thenReturn(mockRestrictions);

        // Execute method
        staXCallbackImpl.doInXMLStream(xtw);

        // Verify interactions
        verify(rsDao, times(1)).fetchRestrictions(any(), any());

        // Validate XML output
        String xmlOutput = output.toString();
        assertTrue(xmlOutput.contains("<restrictedSecurityList>"));
        assertTrue(xmlOutput.contains("RIC001"));
        assertTrue(xmlOutput.contains("RIC002"));
        assertTrue(xmlOutput.contains("</restrictedSecurityList>"));
    }

    /***
     * Test case for handling SQLException in doInXMLStream
     */
    @Test
    void testDoInXMLStream_SQLException() throws Exception {
        // Mock `fetchRestrictions` to throw SQLException
        when(rsDao.fetchRestrictions(any(), any())).thenThrow(new SQLException("Database error"));

        // Expect RuntimeException due to SQLException
        RuntimeException thrown = assertThrows(RuntimeException.class, () -> {
            staXCallbackImpl.doInXMLStream(xtw);
        });

        assertEquals("java.sql.SQLException: Database error", thrown.getMessage());

        // Verify interactions
        verify(rsDao, times(1)).fetchRestrictions(any(), any());
    }

    /***
     * Test case for handling incorrect RICs scenario
     */
    @Test
    void testDoInXMLStream_IncorrectRICs() throws Exception {
        List<RestrictedSecurity> mockRestrictions = new ArrayList<>();
        mockRestrictions.add(new RestrictedSecurity("RIC001"));
        mockRestrictions.add(new RestrictedSecurity("RIC002"));

        Set<String> incorrectRICs = new HashSet<>();
        incorrectRICs.add("RIC003");

        when(rsDao.fetchRestrictions(any(), any())).thenReturn(mockRestrictions);

        // Execute method
        staXCallbackImpl.doInXMLStream(xtw);

        // Verify interactions
        verify(rsDao, times(1)).fetchRestrictions(any(), any());

        // Validate XML output for incorrect RICs
        String xmlOutput = output.toString();
        assertTrue(xmlOutput.contains("<restrictedSecurityList>"));
        assertTrue(xmlOutput.contains("RIC001"));
        assertTrue(xmlOutput.contains("RIC002"));
        assertFalse(xmlOutput.contains("RIC003")); // Incorrect RICs should not be included in valid records
    }
}
