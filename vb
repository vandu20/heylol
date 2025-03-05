import static org.mockito.Mockito.*;
import static org.junit.jupiter.api.Assertions.*;

import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.MockitoAnnotations;

import javax.xml.stream.XMLStreamException;
import java.io.ByteArrayOutputStream;
import java.io.OutputStream;

class StaxTemplateImplTest {

    @Mock
    private StaXCallbackImpl rsReportGenerator;

    @InjectMocks
    private StaxTemplateImpl staxTemplate;

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    void testGenerateXML_Success() {
        OutputStream outputStream = new ByteArrayOutputStream();

        assertDoesNotThrow(() -> {
            staxTemplate.generateXML(outputStream);
        });

        verify(rsReportGenerator, times(1)).doInXMLStream(any());
    }

    @Test
    void testGenerateXML_NullOutputStream() {
        Exception exception = assertThrows(NullPointerException.class, () -> {
            staxTemplate.generateXML(null);
        });

        assertEquals("OutputStream is not initialized", exception.getMessage());
    }

    @Test
    void testGenerateXML_XMLStreamExceptionHandling() throws XMLStreamException {
        doThrow(new XMLStreamException("Test Exception")).when(rsReportGenerator).doInXMLStream(any());

        OutputStream outputStream = new ByteArrayOutputStream();

        assertDoesNotThrow(() -> {
            staxTemplate.generateXML(outputStream);
        });

        verify(rsReportGenerator, times(1)).doInXMLStream(any());
    }
}
