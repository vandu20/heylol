package com.db.fusion.rs.service;

import static org.junit.jupiter.api.Assertions.*;
import static org.mockito.Mockito.*;

import com.db.fusion.rs.dao.RSDaoImpl;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.extension.ExtendWith;
import org.mockito.InjectMocks;
import org.mockito.Mock;
import org.mockito.junit.jupiter.MockitoExtension;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;

import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.sql.Connection;
import java.sql.SQLException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.nio.file.Paths;

@ExtendWith(MockitoExtension.class)
class GenerateRSReportServiceImplTest {

    private static final Logger LOGGER = LoggerFactory.getLogger(GenerateRSReportServiceImplTest.class);

    @Mock
    private Connection connection;

    @Mock
    private RSExtractorService rsExtractor;

    @Mock
    private RSDaoImpl rsDao;

    @InjectMocks
    private GenerateRSReportServiceImpl generateRSReportService;

    private static final String TEST_FILE_PATH = "src/main/resources/restricted_securities.xml";
    
    @BeforeEach
    void setUp() {
        // Ensure test files are deleted before each test
        try {
            Path path = Paths.get(TEST_FILE_PATH);
            Files.deleteIfExists(path);
        } catch (IOException e) {
            LOGGER.error("Error cleaning up test files: " + e.getMessage(), e);
        }
    }

    @Test
    void testGenerateRSReport_Success() throws SQLException, IOException {
        // Arrange
        String mockRestrictionType = "Restricted";
        when(rsDao.fetchRestrictionType(connection)).thenReturn(mockRestrictionType);

        ByteArrayOutputStream mockOutputStream = new ByteArrayOutputStream();
        mockOutputStream.write("Test XML Data".getBytes());
        when(rsExtractor.extract(eq(mockRestrictionType), any())).thenReturn(mockOutputStream);

        // Act
        String resultFilePath = generateRSReportService.generateRSReport();

        // Assert
        assertNotNull(resultFilePath);
        assertEquals(TEST_FILE_PATH, resultFilePath);
        assertTrue(Files.exists(Paths.get(TEST_FILE_PATH)));

        // Verify interactions
        verify(rsDao).fetchRestrictionType(connection);
        verify(rsExtractor).extract(eq(mockRestrictionType), any());
    }

    @Test
    void testGenerateRSReport_ThrowsSQLException() throws SQLException {
        // Arrange
        when(rsDao.fetchRestrictionType(connection)).thenThrow(new SQLException("DB error"));

        // Act & Assert
        RuntimeException thrown = assertThrows(RuntimeException.class, () -> generateRSReportService.generateRSReport());
        assertEquals("java.sql.SQLException: DB error", thrown.getMessage());

        // Verify that extraction was never called
        verify(rsExtractor, never()).extract(anyString(), any());
    }

    @Test
    void testGenerateRSReport_ThrowsIOException() throws SQLException, IOException {
        // Arrange
        String mockRestrictionType = "Restricted";
        when(rsDao.fetchRestrictionType(connection)).thenReturn(mockRestrictionType);
        
        ByteArrayOutputStream mockOutputStream = new ByteArrayOutputStream();
        mockOutputStream.write("Test XML Data".getBytes());
        when(rsExtractor.extract(eq(mockRestrictionType), any())).thenReturn(mockOutputStream);

        // Simulate an IOException when saving the file
        Path mockPath = mock(Path.class);
        when(mockPath.getParent()).thenReturn(mockPath);
        doThrow(new IOException("File write error")).when(Files.class);
        Files.copy(any(), any(), any());

        // Act & Assert
        assertDoesNotThrow(() -> generateRSReportService.generateRSReport());

        // Verify interactions
        verify(rsDao).fetchRestrictionType(connection);
        verify(rsExtractor).extract(eq(mockRestrictionType), any());
    }
}
