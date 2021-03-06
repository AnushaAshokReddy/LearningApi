#install.packages("rgl")
#install.packages("plotrix")
library(rgl)
library(plotrix)

# functionsPlots is a function that plots 2d/3d functions. It loads the desired file, plots functions from startFunction to endFunction
#   filepath: path to file containing the functions
#   functionDimensions: 2 or 3 for 2d and 3d respectively
#   startFunction: the number of the first function to plot from file
#   endFunction: the number of the last function to plot from file
functionsPlots = function(filepath, functionDimensions, startFunction, endFunction) {

    # creating different colors
    color = rainbow(endFunction - startFunction + 1)

    # load all functions from file
    myFunctions = read.csv(file = filePath, header = FALSE, sep = ";")

    # 2 dimensions
    if (functionDimensions == 2) {
        # get x-coordinates and y-coordinates
        for (i in startFunction:endFunction) {
            x = Vectorize(myFunctions[2 * i - 1,])
            y = Vectorize(myFunctions[2 * i,])
            # plot
            if (i == startFunction) {
                # new 2d plot
                par(mar = c(5, 5, 5, 10))
                plot.default(x, y, type = "l", lwd = "2", col = color[i - startFunction + 1])
            }
            else {
                # add to 2d plot
                lines.default(x, y, type = "l", lwd = "2", col = color[i - startFunction + 1])
            }
        }
        # add legend
        legend("topright", legend = paste('Function', startFunction:endFunction), pch = 16, col = color, inset = c(-0.30, 0), xpd = TRUE)
    }
    # 3 dimensions
    if (functionDimensions == 3) {
        # get x-coordinates, y-coordinates and �z-coordinates
        for (i in startFunction:endFunction) {
            x = Vectorize(myFunctions[3 * i - 2,])
            y = Vectorize(myFunctions[3 * i - 1,])
            z = Vectorize(myFunctions[3 * i,])
            # plot
            if (i == startFunction) {
                # new 3d plot
                par3d(windowRect = c(100, 100, 700, 700))
                plot3d(x, y, z, type = "l", lwd = "2", col = color[i - startFunction + 1])
            }
            else {
                # add to 3d plot
                lines3d(x, y, z, lwd = "2", col = color[i - startFunction + 1])
            }
        }
        # add legend
        legend3d("topright", legend = paste('Function', startFunction:endFunction), pch = 16, col = color)
    }
}

# centroidsPlots is a function that plots 2d/3d centroids of similar functions.
#   filepath: path to file containing the functions
#   numClusters: number of clusters used in training
centroidsPlots = function(filepath, numClusters) {

    # creating different colors
    color = rainbow(numClusters)

    # load all functions from file
    myCentroids = read.csv(file = filePath, header = FALSE, sep = ";")

    numFunctions = NROW(myCentroids) / numClusters
    dimensions = NCOL(myCentroids)

    # 2 dimensions
    if (dimensions == 2) {
        # get x-coordinates and y-coordinates
        x = myCentroids$V1
        y = myCentroids$V2
        # set color vector
        colV = c(NROW(myCentroids))
        for (i in 1:NROW(myCentroids)) {
            colV[i] = color[numClusters - (i %% numClusters)]
        }
        # new 2d plot
        par(mar = c(5, 5, 5, 10))
        plot.default(x, y, type = "p", col = colV[1:NROW(myCentroids)])
        # add legend
        legend("topright", legend = paste('Cluster', 1:numClusters), pch = 16, col = colV[1:numClusters], inset = c(-0.30, 0), xpd = TRUE)
    }
    # 3 dimensions
    if (dimensions == 3) {
        # get x-coordinates, y-coordinates and z-coordinates
        x = myCentroids$V1
        y = myCentroids$V2
        z = myCentroids$V3
        # set color vector
        colV = c(NROW(myCentroids))
        for (i in 1:NROW(myCentroids)) {
            colV[i] = color[numClusters - (i %% numClusters)]
        }
        # new 3d plot
        par3d(windowRect = c(100, 100, 700, 700))
        plot3d(x, y, z, type = "p", col = colV[1:NROW(myCentroids)])

        # add legend
        legend3d("topright", legend = paste('Cluster', 1:numClusters), pch = 16, col = colV[1:numClusters])
    }   
}

clustersAndCentroidsPlots = function(fileDirectory, numTestFunctions) {

    clusterCentroids = read.csv(file = paste(fileDirectory, "Calculated Centroids.csv", sep = ""), header = FALSE, sep = ";")
    distances = read.csv(file = paste(fileDirectory, "Calculated Max Distance.csv", sep = ""), header = FALSE, sep = ";")
    myCentroids = read.csv(file = paste(fileDirectory, "Testing Centroids.csv", sep = ""), header = FALSE, sep = ";")

    numClusters = NROW(clusterCentroids)
    dimension = NCOL(distances)
    numDiffFunc = (NROW(myCentroids) / numClusters) / numTestFunctions

    # creating different colors
    color = rainbow(numDiffFunc + 1)

    # set color vector
    colV = c(1:NROW(myCentroids))
    for (i in 1:NROW(myCentroids)) {
        
        colV[i] = color[i%/%(numClusters*numTestFun)+2]
    }

    if (dimension == 2) {
        # new 2d plot
        # plot testing centroids
        x = myCentroids$V1
        y = myCentroids$V2
        par(mar = c(5, 5, 5, 15))
        plot.default(x, y, type = "p", col = colV[1:NROW(myCentroids)])
        # get x-coordinates and y-coordinates of cluster centroids
        x = clusterCentroids$V1
        y = clusterCentroids$V2
        d = Vectorize(distances)       
        # plot cluster centroids        
        points.default(x, y, type = "p", col = color[1], pch = 20)
        # plot clusters
        for (i in 1:numClusters) {
            draw.circle(x[i], y[i], distances[1, i], border = color[1])
        }
        # add legend
        leg = character(numDiffFunc + 1)
        leg[1] = "Clusters of Function 1"
        for (i in 1:numDiffFunc) {
            leg[i+1] = paste("Testing centroids of Function", i)
        }
        legend("topright", legend = leg, pch = 16, col = color, inset = c(-0.80, 0), xpd = TRUE)
    }

    # not working yet for 3d
    if (false) {
        # new 3d plot
        # plot testing centroids
        x = myCentroids$V1
        y = myCentroids$V2
        z = myCentroids$V3
        par3d(windowRect = c(100, 100, 700, 700))
        plot3d(x, y, z, type = "p", col = colV[1:NROW(myCentroids)])
        # get x-coordinates and y-coordinates of cluster centroids
        x = clusterCentroids$V1
        y = clusterCentroids$V2
        z = clusterCentroids$V3
        d = Vectorize(distances)
        # new 2d plot
        # plot cluster centroids
        points3d(x, y, type = "p", col = color[1], pch = 20)
        # plot clusters
        for (i in 1:numClusters) {
            #rgl.spheres(x[i], y[i], z[i], distances[1, i], color = color[1])
        }
    }
}

FunctionDetectionResultsPlots = function(fileDirectory, numTestFunctions) {

    results = read.csv(file = paste(fileDirectory, "Results.csv", sep = ""), header = FALSE, sep = ";")
    res = Vectorize(results)

    numTotalFunc = NCOL(results)
    numDiffFunc = numTotalFunc / numTestFunctions

    # creating different colors
    color = rainbow(numDiffFunc)

    # set color vector
    colV = c(numTotalFunc)
    x = c(numTotalFunc)
    y = c(numTotalFunc)

    for (i in 1:numTotalFunc) {
        x[i] = i%% numTestFunctions
        y[i] = res[1,i]
        colV[i] = color[i %/% numTestFunctions + 1]
    }
    
    # new 2d plot
    par(mar = c(5, 5, 5, 10))
    plot.default(x[1:numTestFunctions], y[1:numTestFunctions], type = "l", ylim = c(-0.5, 1.5), lwd = "2", col = color[1])
    for (i in 2:numDiffFunc) {
        lines.default(x[(i - 1) * numTestFunctions + 1:numTestFunctions * i], y[(i - 1) * numTestFunctions + 1:numTestFunctions * i], type = "l", lwd = "2", col = color[i])
    }

    # add legend
    legend("topright", legend = paste('Function', 1:numDiffFunc), pch = 16, col = color, inset = c(-0.30, 0), xpd = TRUE)
}

TwoFunctionsClustersPlot = function(fileDirectory, f1Name, f2Name, numClusters) {
    clusters1 = read.csv(file = paste(fileDirectory, f1Name, "/Calculated Centroids C", numClusters, ".csv", sep = ""), header = FALSE, sep = ";")
    distances1 = read.csv(file = paste(fileDirectory, f1Name, "/Calculated Max Distance C", numClusters, ".csv", sep = ""), header = FALSE, sep = ";")
    clusters2 = read.csv(file = paste(fileDirectory, f2Name, "/Calculated Centroids C", numClusters, ".csv", sep = ""), header = FALSE, sep = ";")
    distances2 = read.csv(file = paste(fileDirectory, f2Name, "/Calculated Max Distance C", numClusters, ".csv", sep = ""), header = FALSE, sep = ";")

    # creating different colors
    color = rainbow(2)

    dimension = NCOL(clusters1)

    if (dimension == 2) {
        # get x-coordinates and y-coordinates of cluster centroids
        x = clusters1$V1
        y = clusters1$V2
        x2 = clusters2$V1
        y2 = clusters2$V2
        par(mar = c(5, 5, 5, 10))
        # plot cluster centroids        
        plot.default(x, y, type = "p", col = color[1], pch = 20)
        points.default(x2, y2, type = "p", col = color[2], pch = 20)
        # plot clusters
        for (i in 1:numClusters) {
            draw.circle(x[i], y[i], distances1[1, i], border = color[1])
            draw.circle(x2[i], y2[i], distances2[1, i], border = color[2])
        }
        names = c(f1Name,f2Name)
        legend("topright", legend = names[1:2], pch = 16, col = color, inset = c(-0.30, 0), xpd = TRUE)
        
    }

    if (dimension == 3) {
        # get x-coordinates and y-coordinates of cluster centroids
        x = clusters1$V1
        y = clusters1$V2
        z = clusters1$V3
        x2 = clusters2$V1
        y2 = clusters2$V2
        z2 = clusters1$V3
        par3d(windowRect = c(100, 100, 700, 700))
        # plot clusters
        for (i in 1:numClusters) {
            plot3d(x[i], y[i], z[i], type = "s", col = color[1], add = TRUE, radius = distances1[1, i])
            plot3d(x2[i], y2[i], z2[i], type = "s", col = color[2], add = TRUE, radius = distances2[1, i])
            
        }
        decorate3d(xlab = "x", ylab = "y", zlab = "z", box = TRUE, axes = TRUE, main = NULL, sub = NULL, top = TRUE, aspect = TRUE)
        # add legend
        #legend3d("topright", legend = names[1:2], pch = 16, col = color)
    }
}

sourceDir = getSrcDirectory(functionsPlots)
paste(sourceDir)


### Plot a Function

# settings for plotting similar functions
functionName = "COS_COS X"
filePath = paste(sourceDir, "/TestFiles/Functions/", functionName, "/NRP5-10/", functionName, " SimilarFunctions NRP5-10.csv", sep = "")
functionDimensions = 3
startFunction = 1
endFunction = 1

# plot the function
functionsPlots(filepath, functionDimensions, startFunction, endFunction)

### Plot centroids

# settings for plotting centroids of similar functions
numClusters = 2
filePath = paste(sourceDir, "/TestFiles/Functions/", functionName, "/NRP5-10/", functionName, " SimilarFunctions Centroids NRP5-10 KA2 C", numClusters, " I500 R1.csv", sep = "")
# plot the centroids
centroidsPlots(filePath, numClusters)

### Plot clusters and Testing Centroids

# settings
fileDir = paste(sourceDir, "/TestFiles/Function Recognition/",sep = "")
numTestFun = 200
# clustersAndCentroidsPlots(fileDir, numTestFun)
#FunctionDetectionResultsPlots(fileDir, numTestFun)

TwoFunctionsClustersPlot(fileDir, "SIN X", "Triangular", 3)
#TwoFunctionsClustersPlot(fileDir, "SIN_SIN X", "SIN_COS X", 3)