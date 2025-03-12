import static org.mockito.Mockito.*;
import static org.junit.jupiter.api.Assertions.*;

import java.io.*;
import java.nio.file.*;
import org.junit.jupiter.api.*;
import org.junit.jupiter.api.io.TempDir;
import org.mockito.*;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

class GenerateRSReportServiceImplTest {

    @Mock
    private RSExtractorService rsExtractor;

    @Mock
    private RSDaoImpl rsDao;

    @InjectMocks
    private GenerateRSReportServiceImpl generateRSReportService;

    @TempDir
    Path tempDir;  // Temporary directory for testing file writes

    private static final Logger LOGGER = LoggerFactory.getLogger(GenerateRSReportServiceImplTest.class);

    @BeforeEach
    void setUp() {
        MockitoAnnotations.openMocks(this);
    }

    @Test
    void testGenerateRSReport() throws Exception {
        // Mock extractor output
        ByteArrayOutputStream mockOutputStream = new ByteArrayOutputStream();
        mockOutputStream.write("<restricted_securities></restricted_securities>".getBytes());
        when(rsExtractor.extract()).thenReturn(mockOutputStream);

        // Call the method
        String filePath = generateRSReportService.generateRSReport();

        // Verify file creation
        Path reportPath = Paths.get(filePath);
        assertTrue(Files.exists(reportPath), "Report file should exist");

        // Read and verify file content
        String content = Files.readString(reportPath);
        assertEquals("<restricted_securities></restricted_securities>", content.trim(), "File content should match");

        // Verify logging
        LOGGER.info("Generated report is at: " + filePath);
    }

    @Test
    void testGenerateRSReport_ExceptionHandling() throws Exception {
        when(rsExtractor.extract()).thenThrow(new IOException("Mocked IOException"));

        assertThrows(RuntimeException.class, () -> generateRSReportService.generateRSReport());

        // Verify logging
        LOGGER.error("Exception occurred while generating the report");
    }
}
