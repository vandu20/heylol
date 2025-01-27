package com.example.dbcruise.service;

import com.example.dbcruise.dto.PostResponse;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.web.client.RestTemplate;
import java.util.Arrays;
import java.util.List;

@Service
public class DbCruiseService {
    private final RestTemplate restTemplate;

    @Value("${dbcruise.service.url}")
    private String serviceUrl;

    public DbCruiseService(RestTemplate restTemplate) {
        this.restTemplate = restTemplate;
    }

    public List<PostResponse> getFilteredPosts() {
        try {
            // Fetching the response and mapping it to PostResponse[]
            PostResponse[] response = restTemplate.getForObject(serviceUrl, PostResponse[].class);

            // Returning as a list
            return Arrays.asList(response);
        } catch (Exception e) {
            throw new RuntimeException("Error fetching posts: " + e.getMessage(), e);
        }
    }
}
