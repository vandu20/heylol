package com.db.fusion.rs.quorumxml;

import java.sql.*;
import javax.xml.stream.XMLStreamException;
import javax.xml.stream.XMLStreamWriter;
import org.apache.logging.log4j.Logger;
import org.apache.logging.log4j.LogManager;

public class QuorumXMLGenerator {

    private static final Logger LOGGER = LogManager.getLogger(QuorumXMLGenerator.class);

    private static final String URL = "jdbc:oracle:thin:@your_quorum_host:1521:your_db";
    private static final String USER = "your_username";
    private static final String PASSWORD = "your_password";

    private static final String SQL_RESTRICTION_TYPE = "SELECT DISTINCT restriction_type FROM restriction WHERE restriction_code = ?";
    private static final String SQL_RESTRICTIONS = "SELECT restriction_id, security_name, instrument_id_type, instrument_id_value " +
            "FROM restriction WHERE status_code = ? AND restriction_type_code = ?";

    public void generateXMLReport(String restrictionCode) {
        try (Connection connection = DriverManager.getConnection(URL, USER, PASSWORD)) {
            fetchRestrictionTypeAndGenerateReport(connection, restrictionCode);
            fetchRestrictionsAndGenerateReport(connection, restrictionCode);
        } catch (SQLException | XMLStreamException e) {
            LOGGER.error("Error generating XML report: ", e);
        }
    }

    private void fetchRestrictionTypeAndGenerateReport(Connection connection, String restrictionCode) throws SQLException, XMLStreamException {
        try (PreparedStatement preparedStatement = connection.prepareStatement(SQL_RESTRICTION_TYPE)) {
            preparedStatement.setString(1, restrictionCode);
            ResultSet resultSet = preparedStatement.executeQuery();
            while (resultSet.next()) {
                String restrictionType = resultSet.getString(1);
                LOGGER.info("Fetched Restriction Type: " + restrictionType);
                generateReportXML(restrictionType);
            }
        }
    }

    private void fetchRestrictionsAndGenerateReport(Connection connection, String restrictionCode) throws SQLException, XMLStreamException {
        try (PreparedStatement preparedStatement = connection.prepareStatement(SQL_RESTRICTIONS)) {
            preparedStatement.setString(1, "Active"); // Example filter: change as needed
            preparedStatement.setString(2, restrictionCode);
            ResultSet resultSet = preparedStatement.executeQuery();

            while (resultSet.next()) {
                String restrictionId = resultSet.getString("restriction_id");
                String securityName = resultSet.getString("security_name");
                String instrumentIdType = resultSet.getString("instrument_id_type");
                String instrumentIdValue = resultSet.getString("instrument_id_value");

                LOGGER.info("Fetched Data - Restriction ID: " + restrictionId + ", Security Name: " + securityName);

                // Generate XML for restricted security data
                addRestrictedSecurityToXML(restrictionId, securityName, instrumentIdType, instrumentIdValue);
            }
        }
    }

    private void generateReportXML(String restrictionType) throws XMLStreamException {
        LOGGER.debug("Starting XML generation for restriction type: " + restrictionType);

        XMLStreamWriter xmlWriter = new CustomXMLStreamWriter(); // Custom stream writer to output XML to a file or console
        xmlWriter.writeStartElement("restrictedSecurityList");

        // Add report details
        xmlWriter.writeStartElement("reportDetails");
        xmlWriter.writeStartElement("typeOfRestriction");
        xmlWriter.writeCharacters(restrictionType);
        xmlWriter.writeEndElement(); // End of typeOfRestriction
        xmlWriter.writeEndElement(); // End of reportDetails

        // Add restricted securities
        generateRestrictedSecuritiesList(xmlWriter);

        xmlWriter.writeEndElement(); // End of restrictedSecurityList
        xmlWriter.flush();
        xmlWriter.close();

        LOGGER.info("XML Report generation completed for restriction type: " + restrictionType);
    }

    private void generateRestrictedSecuritiesList(XMLStreamWriter xmlWriter) throws XMLStreamException {
        // Example of adding a single restricted security to the XML
        xmlWriter.writeStartElement("restrictedSecurity");

        // Add security description (You can pull this from the database or set it as a constant)
        xmlWriter.writeStartElement("securityDescription");
        xmlWriter.writeCharacters("Example Security Name");
        xmlWriter.writeEndElement(); // End of securityDescription

        // Example of adding multiple identifiers (ISIN, CUSIP, RIC, WPK)
        addSecurityIdentifier(xmlWriter, "ISIN", "US1234567890");
        addSecurityIdentifier(xmlWriter, "CUSIP", "123456789");
        addSecurityIdentifier(xmlWriter, "RIC", "XYZ123");
        addSecurityIdentifier(xmlWriter, "WPK", "WPK123");

        xmlWriter.writeEndElement(); // End of restrictedSecurity
    }

    private void addSecurityIdentifier(XMLStreamWriter xmlWriter, String identifierType, String identifierValue) throws XMLStreamException {
        xmlWriter.writeStartElement("securityIdentifier");

        // Add securityNumberingAgencyCode
        xmlWriter.writeStartElement("securityNumberingAgencyCode");
        xmlWriter.writeCharacters(identifierType);
        xmlWriter.writeEndElement(); // End of securityNumberingAgencyCode

        // Add securityIdentifier
        xmlWriter.writeStartElement("securityIdentifier");
        xmlWriter.writeCharacters(identifierValue);
        xmlWriter.writeEndElement(); // End of securityIdentifier

        xmlWriter.writeEndElement(); // End of securityIdentifier element
    }

    // Custom XMLStreamWriter to write to a file or stream (assumed implementation)
    private class CustomXMLStreamWriter implements XMLStreamWriter {
        @Override
        public void writeStartElement(String localName) throws XMLStreamException {
            System.out.println("<" + localName + ">");
        }

        @Override
        public void writeEndElement() throws XMLStreamException {
            System.out.println("</>");
        }

        @Override
        public void writeCharacters(String text) throws XMLStreamException {
            System.out.println(text);
        }

        @Override
        public void flush() throws XMLStreamException {
            // Implement flush logic
        }

        @Override
        public void close() throws XMLStreamException {
            // Implement close logic
        }

        // Implement other required methods from XMLStreamWriter interface
    }
}
