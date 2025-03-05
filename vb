import static org.mockito.Mockito.*;
import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import javax.xml.stream.XMLStreamException;
import javax.xml.stream.XMLStreamWriter;
import java.util.Arrays;
import java.util.List;

class StaXCallbackImplTest {

    @Mock
    private RSDao rsDao;

    @InjectMocks
    private StaXCallbackImpl staXCallback;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    void testDoInXMLStream_Success() throws XMLStreamException {
        XMLStreamWriter xtw = mock(XMLStreamWriter.class);
        List<RestrictedSecurity> mockSecurities = Arrays.asList(new RestrictedSecurity());

        when(rsDao.fetchRestrictions(any(), any())).thenReturn(mockSecurities);

        assertDoesNotThrow(() -> staXCallback.doInXMLStream(xtw));

        verify(xtw, times(1)).writeStartDocument("UTF-8", "1.0");
        verify(xtw, times(1)).writeEndDocument();
    }

    @Test
    void testDoInXMLStream_NullXMLStreamWriter() {
        Exception exception = assertThrows(NullPointerException.class, () -> {
            staXCallback.doInXMLStream(null);
        });

        assertEquals("XMLStreamWriter is not initialized", exception.getMessage());
    }

    @Test
    void testDoInXMLStream_HandlesSQLException() throws Exception {
        XMLStreamWriter xtw = mock(XMLStreamWriter.class);

        when(rsDao.fetchRestrictions(any(), any())).thenThrow(new RuntimeException("DB Error"));

        assertDoesNotThrow(() -> staXCallback.doInXMLStream(xtw));
    }
}
